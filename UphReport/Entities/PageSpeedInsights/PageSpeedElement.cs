using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace UphReport.Entities.PageSpeedInsights
{
    public enum TypeOfResult
    {
        ERROR,
        WARNING,
        PASSED
    }

    public class PageSpeedElement
    {
        [Key]
        public Guid Id { get; set; }
        public string ElementName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TypeOfResult TypeOfResult { get; set; }

        public Guid PageSpeedReportId { get; set; }

        public virtual PageSpeedReport PageSpeedReport{ get; set; }
        public virtual List<PageSpeedSubElement> PageSpeedSubElement { get; set; }

        public PageSpeedElement()
        {

        }

    }
}
