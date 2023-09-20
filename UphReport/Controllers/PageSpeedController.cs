using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UphReport.Entities.PageSpeedInsights;
using UphReport.Interfaces;
using UphReport.Models.PSI;

namespace UphReport.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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
	public async Task<IActionResult> GetReportsAsync([FromBody]PageSpeedRequest pageSpeedRequest)
	{
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        var result = await _service.GetReportsAsync(pageSpeedRequest, token);
		return Ok(result);
	}

    [HttpPost("reportDomain")]
    public async Task<IActionResult> GetReportsDomainAsync([FromBody] PageSpeedRequestDomain pageSpeedRequestDomain)
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

    [HttpPost("multiSave")]
    public async Task<IActionResult> SaveMultiReportAsync(List<PageSpeedReport> pageSpeedReports)
    {
		var result = await _service.SaveMultiReport(pageSpeedReports);
        return Ok(result);
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
    [HttpGet("getOne")]
    public async Task<IActionResult> GetOne([FromQuery] Guid webLinksId)
    {
        var result = await _service.GetOneReport(webLinksId);
        return Ok(result);
    }
    [HttpGet("multiple")]
	public async Task<IActionResult> GetMultipleReportAsync([FromQuery]string domain)
	{
		var result = await _service.GetMultipleReport(domain);
		return Ok(result);
	}
	[HttpGet("linksAndReports")]
    public async Task<IActionResult> GetLinksAndReportsAsync([FromQuery] string domain, [FromQuery] int strategy)
    {
        var result = await _service.GetLinksAndReportAsync(domain, strategy);
        return Ok(result);
    }

	[HttpGet("getByUser")]
	public async Task<IActionResult> GetReportsByUser([FromQuery]int id)
	{
		var response = await _service.GetMultipleReportByUser(id);
		return Ok(response);
	}
	[HttpGet("getDomain")]
	public async Task<IActionResult> GetDomainName([FromQuery] Guid guid)
	{
		var response = await _service.GetDomainByReportId(guid);

		return Ok(response);
	}
}
