using UphReport.Entities.PageSpeedInsights;

namespace UphReport.Entities.Wave;

public class WaveReport
{
    public Guid Id { get; set; }
    public string WebName { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int? CreatedById { get; set; }
    public Strategy Strategy { get; set; }
    public string? WaveVersion { get; set; }
    public string? SystemVersion { get; set; }

    public Guid WebPageId { get; set; }

    public virtual User CreatedBy { get; set; }

    public virtual List<WaveElement> WaveElements { get; set; }
}
