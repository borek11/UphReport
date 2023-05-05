using System;

namespace UphReport.Entities.PageSpeedInsights
{
    public class PageSpeedSubElement
    {
        public Guid Id { get; set; }
        public string Snippet { get; set; }
        public string Selector { get; set; }
        public Guid PageSpeedElementId { get; set; }

        public virtual PageSpeedElement PageSpeedElement { get; set; }

        public PageSpeedSubElement()
        {

        }
    }
}
