using UphReport.Entities.PageSpeedInsights;

namespace UphReport.Models.PSI;

public class PageSpeedMultiReportResponse
{
    public Guid Id { get; set; }
    public string WebName { get; set; }
    public float Result { get; set; }
    public int AmountOfErrors { get; set; }
    public int AmountOfPassed { get; set; }
    public DateTime DateTime { get; set; }
    public Strategy Strategy { get; set; }

    public PageSpeedMultiReportResponse()
    {

    }
}
