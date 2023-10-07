using Microsoft.AspNetCore.Identity;
using Shared.DTO;

namespace Service.Contracts;

public interface IAuthenticationService
{
    Task<TokenDTO> ValidateUserAndCreateToken(LoginUserDTO request);
    Task<TokenDTO> RefreshToken(TokenDTO tokenDto);
}