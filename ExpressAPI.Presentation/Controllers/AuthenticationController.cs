using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Identity;
using Shared.DTO;

namespace ExpressAPI.Presentation.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IServiceManager _service;

    public AuthenticationController(IServiceManager service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDTO request)
    {
        var tokenDto = await _service.AuthenticationService.ValidateUserAndCreateToken(request);
        if (tokenDto is not null)
            return Ok(tokenDto);

        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenDTO tokenDto)
    {
        var tokenDtoToReturn = await _service.AuthenticationService.RefreshToken(tokenDto);
        return Ok(tokenDtoToReturn);
    }

}