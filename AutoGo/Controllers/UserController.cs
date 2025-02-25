using AutoGo.Services;
using AutoGo.Services.Exceptions;
using AutoGo.Services.Model;
using Microsoft.AspNetCore.Mvc;

namespace AutoGo.Controllers;
[ApiController]
public class UserController(ILogger<UserController> logger, IUserService userService) : ControllerBase
{
    [HttpPost("user/addDriver")]
    public async Task<IActionResult> AddDriver(AddDriverModel addDriver)
    {
        try
        {
            await userService.AddDriver(addDriver);
            return Ok("User Created");
        }
        catch (BadDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError("Something went wrong while driver creation, {ex}", ex);
            return BadRequest("Something went wrong");

        }
    }

    [HttpPost("user/Verify")]
    public async Task<IActionResult> Verify(int userId, string Pin)
    {
        try
        {
            await userService.VerifyUser(userId, Pin, 0);
            return Ok("User Created");
        }
        catch (BadDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError("Something went wrong while verifying user, {ex}", ex);
            return BadRequest("Something went wrong");

        }
    }
}
