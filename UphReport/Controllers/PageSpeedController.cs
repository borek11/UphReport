﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UphReport.Entities.PageSpeedInsights;
using UphReport.Interfaces;
using UphReport.Models.PSI;

namespace UphReport.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PageSpeedController : ControllerBase
{
	private readonly IPageSpeedService _service;

	public PageSpeedController(IPageSpeedService service)
	{
		_service = service;
	}
	[HttpPost("generate")]
	public async Task<IActionResult> GenerateRaportAsync([FromQuery(Name = "urlRequest")]string urlRequest,[FromQuery(Name = "strategy")] Strategy strategy)
	{
		var result = await _service.GenerateReportAsync(urlRequest, strategy);
		return Ok(result);
	}

	[HttpPost("report")]
	public async Task<IActionResult> GetReportsAsyns([FromBody]PageSpeedRequest pageSpeedRequest)
	{
		var result = await _service.GetReportsAsync(pageSpeedRequest);
		return Ok(result);
	}

    [HttpPost("reportDomain")]
    public async Task<IActionResult> GetReportsDomainAsyns([FromBody] PageSpeedRequestDomain pageSpeedRequestDomain)
    {
        var result = await _service.GetReportsAsync(pageSpeedRequestDomain);
        return Ok(result);
    }

    [HttpPost("save")]
	public async Task<IActionResult> SaveReportAsync()
	{
		//var result = await _service.SaveReportAsync();
		//if (result)
		//{
		//	return NoContent();
		//}
		//return BadRequest();
		return Ok();
	}
	[HttpGet]
	public async Task<IActionResult> GetReportFromDBAsync(Guid guid)
	{
		var result = await _service.GetReportFromDBAsync(guid);
		return Ok(result);
	}
	[HttpDelete]
	public async Task<IActionResult> DeleteReportAsync(Guid guid)
	{
		await _service.DeleteReportAsync(guid);
		return NoContent();
	}
	[HttpGet("multiple")]
	public async Task<IActionResult> GetMultipleReportAsync([FromQuery]string domain)
	{
		var result = await _service.GetMultipleReport(domain);
		return Ok(result);
	}

}