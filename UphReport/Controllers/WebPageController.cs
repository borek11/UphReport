using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UphReport.Interfaces;
using UphReport.Models.WebPage;

namespace UphReport.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WebPageController : ControllerBase
{
    private readonly IWebPage _service;

    public WebPageController(IWebPage webPageservice)
    {
        _service = webPageservice;
    }
    [HttpPost]
    public async Task<IActionResult> SearchAsync([FromBody] WebPageDto webPageDto)
    {
        return Ok(await _service.SearchUrlsAsync(webPageDto));
    }
    [HttpPost("Save")]
    public async Task<IActionResult> SaveAsync(WebPageRequest webPageRequest)
    {
        var result = await _service.SaveLinksAsync(webPageRequest);
        if (result > 0)
            return Ok(result);

        return NoContent();
    }
    [HttpDelete("{guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute]Guid guid)
    {
        await _service.DeleteLinkAsync(guid);
        return NoContent();
    }
    [HttpDelete("domain")]
    //[Route("domain/{domainName}")]
    public async Task<IActionResult> DeleteAboutDomainAsync([FromQuery(Name = "domainName")] string domainName)
    {
        await _service.DeleteLinksAboutDomainAsync(domainName);
        return NoContent();
    }

}
