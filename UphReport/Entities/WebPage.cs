using System;
using UphReport.Entities.PageSpeedInsights;
using UphReport.Entities.Wave;

namespace UphReport.Entities
{
    public class WebPage
    {
        public Guid Id { get; set; }
        public string WebName { get; set; }
        public string DomainName { get; set; }
        public bool IsCheck { get; set; } = false;
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public virtual List<PageSpeedReport> PageSpeedReport { get; set; }
        public virtual List<WaveReport> WaveReports { get; set; }
        public WebPage()
        {

        }
    }
}
