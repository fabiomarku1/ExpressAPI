using AutoMapper;
using Contracts;
using Cryptography;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Shared.DTO;
using Shared.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Service;

public class AuthenticationService : IAuthenticationService
{
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOptions<JwtConfiguration> _configuration;
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly IRepositoryManager _repositoryManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ICryptoUtils _cryptoUtils;

    public AuthenticationService(ILoggerManager logger, IMapper mapper, UserManager<ApplicationUser> userManager, IOptions<JwtConfiguration> configuration, IRepositoryManager repositoryManager, SignInManager<ApplicationUser> signInManager, ICryptoUtils cryptoUtils)
    {
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
        _configuration = configuration;
        _jwtConfiguration = configuration.Value;
        _repositoryManager = repositoryManager;
        _signInManager = signInManager;
        _cryptoUtils = cryptoUtils;
    }

    public async Task<TokenDTO> ValidateUserAndCreateToken(LoginUserDTO request)
    {
        ApplicationUser currentUser = null;
      
        currentUser = _userManager.Users.FirstOrDefault(u => u.Email == request.Username || u.UserName == request.Username);

        if (currentUser == null)
            throw new BadRequestException("Wong email or password , please try again !");


        var validateUser = await _signInManager.PasswordSignInAsync(currentUser, request.Password, false, lockoutOnFailure: true);

        if (!validateUser.Succeeded)
        {
            _logger.LogWarn(string.Format("Authentication failed!", nameof(ValidateUserAndCreateToken)));

            if (validateUser.IsLockedOut)
                throw new BadRequestException("Please try again later!");

            throw new BadRequestException("Wrong credentials!");
        }

        var tokenDto = await CreateToken(currentUser, true, false);
        return tokenDto;
    }

    public async Task<TokenDTO> RefreshToken(TokenDTO tokenDto)
    {
        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
        var userEmail = principal.Claims.Where(x => x.Type == "Email").FirstOrDefault();
        var userPhoneNumber = principal.Claims.Where(x => x.Type == "PhoneNumber").FirstOrDefault();

        var currentUser = await _userManager.FindByEmailAsync(userEmail is not null ? userEmail.Value : "");

        if (currentUser is null && userPhoneNumber is not null)
            currentUser = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == userPhoneNumber.Value);

        if (currentUser == null || currentUser.RefreshTokenExpiryTime <= DateTime.Now)
            throw new BadRequestException("sd");


        return await CreateToken(currentUser, false, false);
    }

    #region private

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256Signature);
    }

    private async Task<TokenDTO> CreateToken(ApplicationUser? currentUser, bool populateExp, bool rememberMe)
    {
        if (currentUser is not null)
        {
            var signingCredentials = GetSigningCredentials();
            var tokenHash = _cryptoUtils.Encrypt($"{currentUser.Id}{currentUser.Email}{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}");
            var claims = await GetClaims(currentUser, tokenHash);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                NotBefore = DateTime.Now,
                IssuedAt = DateTime.Now,
                Expires = rememberMe ? DateTime.Now.AddMonths(1) : DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
                SigningCredentials = signingCredentials,
                Audience = _jwtConfiguration.ValidAudience,
                Issuer = _jwtConfiguration.ValidIssuer
            };

            var refreshToken = GenerateRefreshToken();
            currentUser.RefreshToken = refreshToken;
            currentUser.TokenHash = tokenHash;

            if (populateExp)
                currentUser.RefreshTokenExpiryTime = rememberMe ? DateTime.Now.AddMonths(2) : DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.RefreshTokenExpire));

            await _userManager.UpdateAsync(currentUser);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            return new TokenDTO(accessToken, refreshToken);
        }

        return new TokenDTO("", "");
    }


    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private async Task<ClaimsIdentity> GetClaims(ApplicationUser currentUser, string tokenHash)
    {

        var claims = new List<Claim>
             {
                new Claim("Id", currentUser.Id.ToString()),
                new Claim("Email", !string.IsNullOrWhiteSpace(currentUser.Email)?currentUser.Email : ""),
                new Claim("PhoneNumber", currentUser.PhoneNumber !=null?currentUser.PhoneNumber  :""),
                new Claim("FirstName", !string.IsNullOrWhiteSpace(currentUser.FirstName)?currentUser.FirstName : ""),
                new Claim("LastName", !string.IsNullOrWhiteSpace(currentUser.LastName)? currentUser.LastName : ""),
                new Claim("TokenHash", tokenHash),
        };

        var roles = await _userManager.GetRolesAsync(currentUser);
        if (roles is null)
            throw new NotFoundException(string.Format("No roles found!", currentUser.Id));
        if (roles.Count == 1)
        {
            claims.Add(new Claim(ClaimTypes.Role, roles[0]));
        }

        return new ClaimsIdentity(claims);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        int refreshTokenExpire = Convert.ToInt32(_jwtConfiguration.RefreshTokenExpire);
        int tokenExpire = Convert.ToInt32(_jwtConfiguration.Expires);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = false,
            ClockSkew = TimeSpan.FromMinutes(refreshTokenExpire - tokenExpire),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey)),
            ValidIssuer = _jwtConfiguration.ValidIssuer,
            ValidAudience = _jwtConfiguration.ValidAudience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        if (principal is null)
            throw new NotFoundException(string.Format("NoPrincipal"));

        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null)
            throw new SecurityTokenException("InvalidToken");

        return principal;
    }

    #endregion
}