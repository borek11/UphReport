using UphReport.Entities.PageSpeedInsights;

namespace UphReport.Models.Wave;

public class WaveMultiReportResponse
{
    public Guid Id { get; set; }
    public string WebName { get; set; }
    public int AmountOfErrors { get; set; }
    public int AmountOfPassed { get; set; }
    public DateTime DateTime { get; set; }
    public Strategy? Strategy { get; set; }
}
