using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTO;

namespace ExpressAPI.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public UserController(IServiceManager service)
    {
        _serviceManager = service;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetUserList()
    {
        var list = await _serviceManager.UserService.GetUsers();
        return Ok(list);
    }


    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO request)
    {
        var result = await _serviceManager.UserService.CreateUser(request);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser( int id)
    {
        var result = await _serviceManager.UserService.GetUserDetails(id);
        return Ok(result);
    }

    [HttpGet("pattern/{searchPattern}")]
    public async Task<IActionResult> GetUserByPattern(string searchPattern)
    {
        var result = await _serviceManager.UserService.GetUsersByPattern(searchPattern);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO request)
    {
        var result = await _serviceManager.UserService.UpdateUser(id,request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _serviceManager.UserService.DeleteUser(id);
        return Ok(result);
    }

}
