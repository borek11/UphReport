using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UphReport.Entities.PageSpeedInsights
{
    public enum Strategy
    {
        DESKTOP,
        MOBILE
    }
    public class PageSpeedReport
    {
        public Guid Id { get; set; }
        public string WebName { get; set; }
        public float Result { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public int? CreatedById { get; set; }
        public Strategy Strategy{ get; set; }
        public string PSIVersion { get; set; }
        public string SystemVersion { get; set; }

        public Guid WebPageId { get; set; }

        public virtual User CreatedBy { get; set; }

        public virtual List<PageSpeedElement> PageSpeedElement { get; set; } 
        public PageSpeedReport()
        {

        }
    }
}
