using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UphReport.Interfaces;
using UphReport.Models.Wave;

namespace UphReport.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WaveAPIKeyController : ControllerBase
{
	private readonly IWaveAPIKeyService _service;

	public WaveAPIKeyController(IWaveAPIKeyService waveAPIKeyService)
	{
        _service = waveAPIKeyService;
	}
	[Authorize(Roles = "Admin")]
	[HttpPost]
	public async Task<IActionResult> AddKey([FromBody]WaveAKRequest waveAKRequest)
	{
		var result = await _service.AddKey(waveAKRequest);
		if (result)
		{
            return Ok();
        }
		else
		{
			return BadRequest("Something went wrong during adding key");
		}
	}
    [Authorize(Roles = "Admin")]
    [HttpGet]
	public async Task<IActionResult> GetKey()
	{
		var result = await _service.GetAPIKey();
		if(result == null)
		{
			return NotFound();
		}
		else
		{
			return Ok(result);
		}
	}
    [Authorize(Roles = "Admin")]
    [HttpGet("getAll")]
	public async Task<IActionResult> GetAllKey()
	{
		return Ok(await _service.GetAll());
	}
    [Authorize(Roles = "Admin")]
    [HttpDelete]
	public async Task<IActionResult> DeleteDeprectedKeys()
	{
		await _service.DeleteDeprecatedKeys();
		return Ok();
	}
    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> UpdateKey(WaveAKUpdate waveAKUpdate)
    {
		await _service.UpdateKey(waveAKUpdate);
		return NoContent();
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("deleteById")]
	public async Task<IActionResult> DeleteById(Guid guid)
	{
		var result = await _service.DeleteById(guid);
		if (!result)
			return NotFound();
		return NoContent();
	}

}
