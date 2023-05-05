using UphReport.Entities.PageSpeedInsights;

namespace UphReport.Entities.Wave;

public class WaveDomainRequests
{
    public string Domain { get; set; }
    public bool Save { get; set; }
    public bool GenerateForExistsReport { get; set; }
    public Strategy Strategy { get; set; }
}
