using UphReport.Entities.PageSpeedInsights;

namespace UphReport.Models.Wave;

public class WaveReportRequest
{
    public string Url { get; set; }
    public string Key { get; set; }
    public Strategy Strategy { get; set; }

    public WaveReportRequest()
    {

    }
}
