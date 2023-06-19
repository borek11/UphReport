using UphReport.Entities.PageSpeedInsights;

namespace UphReport.Models.PSI;

public class PageSpeedAndWebLinks
{
    public Guid Id { get; set; }
    public string WebName { get; set; }
    public string DomainName { get; set; }
    public Guid? ReportId { get; set; }
    public float? Result { get; set; }
    public DateTime? DateTime { get; set; }
    public Strategy? Strategy { get; set; }

    public PageSpeedAndWebLinks()
    {

    }
}
