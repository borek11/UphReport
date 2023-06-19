using UphReport.Entities.PageSpeedInsights;
using UphReport.Models.PSI;

namespace UphReport.Interfaces;

public interface IPageSpeedService
{
    Task<string> GenerateReportAsync(string urlRequest, Strategy strategy);
    Task<PageSpeedReport> ReportCleanAsync(string reportRequest, Strategy strategy);
    Task<Guid> SaveReportAsync(PageSpeedReport pageSpeedReport);
    Task<int> SaveMultiReport(List<PageSpeedReport> pageSpeedReports);
    Task<List<PageSpeedReport>> GetReportsAsync(PageSpeedRequest reportRequest);
    Task<List<PageSpeedReport>> GetReportsAsync(PageSpeedRequestDomain pageSpeedRequestDomain);
    Task<PageSpeedReport> GetReportFromDBAsync(Guid guid);
    Task<bool> DeleteReportAsync(Guid guid);
    Task<List<PageSpeedMultiReportResponse>> GetOneReport(Guid webLinksId);
    Task<List<PageSpeedMultiReportResponse>> GetMultipleReport(string domain);
    Task<List<PageSpeedAndWebLinks>> GetLinksAndReportAsync(string domainName, int strategy);
}
