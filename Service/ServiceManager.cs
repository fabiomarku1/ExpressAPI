using AutoMapper;
using Contracts;
using Cryptography;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Service.Contracts;
using Shared.Utility;

namespace Service;

public class ServiceManager : IServiceManager
{

    private readonly Lazy<IUserService> _userService;
    private readonly Lazy<IAuthenticationService> _authenticationService;
    public ServiceManager(IRepositoryManager repositoryManager
        , ILoggerManager logger
        , IMapper mapper
        , UserManager<ApplicationUser> userManager
        , SignInManager<ApplicationUser> userSignInManager
        , IOptions<JwtConfiguration> jwtConfiguration
        , ICryptoUtils cryptoUtils
    )
    {
        _userService = new Lazy<IUserService>(() => new UserService(logger, mapper, repositoryManager, userManager));
        _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(logger, mapper, userManager, jwtConfiguration, repositoryManager, userSignInManager, cryptoUtils));
    }

    public IUserService UserService => _userService.Value;
    public IAuthenticationService AuthenticationService => _authenticationService.Value;
}