using UphReport.Entities.PageSpeedInsights;

namespace UphReport.Entities.Wave;

public class WaveSubElement
{
    public Guid Id { get; set; }
    public string Selector { get; set; }
    public Guid WaveElementId { get; set; }

    public virtual WaveElement WaveElement { get; set; }
}
