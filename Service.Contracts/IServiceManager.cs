//using Microsoft.AspNetCore.Authentication;

namespace Service.Contracts;

public interface IServiceManager
{
    IUserService UserService { get; }
    IAuthenticationService AuthenticationService { get; }
}