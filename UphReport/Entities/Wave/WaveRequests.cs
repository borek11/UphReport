using UphReport.Entities.PageSpeedInsights;

namespace UphReport.Entities.Wave;

public class WaveRequests
{
    public List<string> Urls { get; set; }
    public bool Save { get; set; }
    public bool GenerateForExistsReport { get; set; }
    public Strategy Strategy { get; set; }
}
