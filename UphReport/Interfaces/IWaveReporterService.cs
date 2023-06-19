using UphReport.Entities.PageSpeedInsights;
using UphReport.Entities.Wave;
using UphReport.Models.PSI;
using UphReport.Models.Wave;

namespace UphReport.Interfaces;

public interface IWaveReporterService
{
    Task<string> GenerateReportAsync(WaveReportRequest waveReportRequest);
    Task<WaveReport> ReportCleanAsync(string reportRequest, Strategy strategy);
    Task<List<WaveReport>> GetReportsAsync(WaveRequests waveRequests);
    Task<List<WaveReport>> GetReportsAsync(WaveDomainRequests waveDomainRequests);
    Task<Guid> SaveReportAsync(WaveReport waveReport);
    Task<WaveReport> GetReportFromDBAsync(Guid guid);
    Task<bool> DeleteReportAsync(Guid guid);
    Task<List<WaveMultiReportResponse>> GetOneReport(Guid webLinksId);
    Task<List<WaveMultiReportResponse>> GetMultipleReport(string domain);
    Task<List<WaveAndWebLinks>> GetLinksAndReportAsync(string domainName, int strategy);

}
