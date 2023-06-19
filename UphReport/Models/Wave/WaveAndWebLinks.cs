using UphReport.Entities.PageSpeedInsights;

namespace UphReport.Models.Wave;

public class WaveAndWebLinks
{
    public Guid Id { get; set; }
    public string WebName { get; set; }
    public string DomainName { get; set; }
    public int? AmountOfErrors { get; set; }
    public int? AmountOfWarnings { get; set; }
    public int? AmountOfPassed { get; set; }
    public Guid? ReportId { get; set; }
    public DateTime? DateTime { get; set; }
    public Strategy? Strategy { get; set; }

    public WaveAndWebLinks()
    {

    }
}
