using UphReport.Entities.PageSpeedInsights;

namespace UphReport.Models.PSI;

public class PageSpeedRequest
{
    public List<string> Urls { get; set; }
    public bool Save { get; set; }
    public bool GenerateForExistsReport { get; set; }
    public Strategy Strategy { get; set; }
    public string DomainName { get; set; }
};
