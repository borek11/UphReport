using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UphReport.Entities.PageSpeedInsights;
using UphReport.Entities.Wave;
using UphReport.Interfaces;
using UphReport.Models.PSI;
using UphReport.Models.Wave;

namespace UphReport.Controllers;
[Route("api/[controller]")]
[ApiController]
public class WaveController : ControllerBase
{
	private readonly IWaveReporterService _service;

	public WaveController(IWaveReporterService service)
	{
		_service = service;
	}

	[HttpPost]
	public async Task<IActionResult> GenerateReportAsync(WaveReportRequest waveReportRequest)
	{
		var result = await _service.GenerateReportAsync(waveReportRequest);
		return Ok(result);
	}



    //[HttpPost("ReportClean")]
    //public async Task<IActionResult> GetReportCleanAsync([FromQuery]string urlRequest)
    //{
    //	var result = await _service.ReportCleanAsync(urlRequest);
    //	return Ok(result);
    //}
    //[HttpPost("generate")]
    //public async Task<IActionResult> GenerateRaportAsync([FromQuery(Name = "urlRequest")] string urlRequest, [FromQuery(Name = "strategy")] Strategy strategy)
    //{
    //    var result = await _service.GenerateReportAsync(urlRequest, strategy);
    //    return Ok(result);
    //}

    [HttpPost("report")]
    public async Task<IActionResult> GetReportsAsync([FromBody] WaveRequests waveRequests)
    {
        var result = await _service.GetReportsAsync(waveRequests);
        return Ok(result);
    }

    [HttpPost("reportDomain")]
    public async Task<IActionResult> GetReportsDomainAsync(WaveDomainRequests waveDomainRequests)
    {
        var result = await _service.GetReportsAsync(waveDomainRequests);
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
    public async Task<IActionResult> GetMultipleReportAsync([FromQuery] string domain)
    {
        var result = await _service.GetMultipleReport(domain);
        return Ok(result);
    }

}
