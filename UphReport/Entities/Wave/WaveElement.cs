using System.ComponentModel.DataAnnotations;
using UphReport.Entities.PageSpeedInsights;

namespace UphReport.Entities.Wave;

public class WaveElement
{
    [Key]
    public Guid Id { get; set; }
    public string ElementName { get; set; }
    public string Description { get; set; }
    public TypeOfResult TypeOfResult { get; set; }

    public Guid WaveReportId { get; set; }

    public virtual WaveReport WaveReport { get; set; }
    public virtual List<WaveSubElement> WaveSubElements { get; set; }

    public WaveElement()
    {

    }
}
