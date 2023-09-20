using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UphReport.Interfaces;
using UphReport.Models.User;
using UphReport.Services;

namespace UphReport.Controllers;
[Route("api/[controller]")]
[ApiController]

public class UserController : ControllerBase
{
    private readonly IUserService _userService;

	public UserController(IUserService userService)
	{
		_userService = userService;
	}

	[HttpPost("register")]
	public async Task<IActionResult> RegisterAsync(RegisterUser registerUser)
	{
		var result = await _userService.RegisterUserAsync(registerUser);
		return Ok();
	}

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginUser loginUser)
    {
        var result = await _userService.LoginAsync(loginUser);
        return Ok(result);
    }
	[HttpGet]
	public async Task<IActionResult> GetUserAsync()
	{
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

		var result = await _userService.GetUserAsync(token);
        return Ok(result);
	}

	[HttpPut("personal")]
	public async Task<IActionResult> UpdateUserAsync(UserRequest userRequest)
	{
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        var result = await _userService.UpdateUserAsync(token, userRequest);

		if(result)
			return Ok();

		return BadRequest();
	}

	[HttpPut("password")]
    public async Task<IActionResult> UpdateUserPasswordAsync(UserPasswordRequest userPasswordRequest)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

		var result = await _userService.UpdatePasswordUserAsync(token, userPasswordRequest);
		if(result)
			return Ok();

		return BadRequest();
    }
	[HttpGet("name")]
	public async Task<IActionResult> GetUserName(int id)
	{
		var result = await _userService.GetNameAsync(id);
		return Ok(result);
	}
	[Authorize(Roles = "Admin")]
	[HttpGet("authAdmin")]
	public IActionResult AuthAdmin()
	{
		return Ok();
	}

    [Authorize(Roles = "User")]
    [HttpGet("authUser")]
    public IActionResult AuthUser()
    {
        return Ok();
    }
    [Authorize(Roles = "Admin,User")]
    [HttpGet("authAll")]
    public IActionResult AuthAll()
    {
        return Ok();
    }

	[Authorize(Roles = "Admin")]
	[HttpGet("getAll")]
	public async Task<IActionResult> GetAllUsers()
	{
		var response = await _userService.GetAllUsers();
		return Ok(response);
	}

    [Authorize(Roles = "Admin")]
    [HttpPost("block")]
    public async Task<IActionResult> BlockUser(int id)
    {
        var response = await _userService.BlockUser(id);
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("unblock")]
    public async Task<IActionResult> UnBlockUser(int id)
    {
        var response = await _userService.UnBlockUser(id);
        return Ok(response);
    }
}
