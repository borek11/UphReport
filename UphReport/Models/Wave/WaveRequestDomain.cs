using UphReport.Entities.PageSpeedInsights;

namespace UphReport.Models.Wave;

public class WaveRequestDomain
{
    public string Domain { get; set; }
    public bool Save { get; set; }
    public bool GenerateForExistsReport { get; set; }
    public Strategy Strategy { get; set; }

    public WaveRequestDomain()
    {

    }
}
