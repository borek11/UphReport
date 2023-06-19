namespace UphReport.Models.WebPage;

public class WebAllPageRequest
{
    public Guid Id { get; set; }
    public string WebName { get; set; }
    public string DomainName { get; set; }
    public DateTime Date { get; set; }

    public WebAllPageRequest()
    {

    }
}
