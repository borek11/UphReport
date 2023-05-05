using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UphReport.Data;
using UphReport.Entities.PageSpeedInsights;
using UphReport.Exceptions;
using UphReport.Interfaces;
using UphReport.Models.PSI;

namespace UphReport.Services
{
    public class PageSpeedReporterService : IPageSpeedService
    {
        private readonly MyDbContext _myDbContext;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IWebPage _webPageService;

        public PageSpeedReporterService(MyDbContext myDbContext, IConfiguration configuration, HttpClient httpClient, IWebPage WebPageService)
        {
            _myDbContext = myDbContext;
            _configuration = configuration;
            _httpClient = httpClient;
            _webPageService = WebPageService;
        }

        public async Task<string> GenerateReportAsync(string urlRequest, Strategy strategy)
        {
            
            //HttpClient httpClient = new HttpClient();
            var requestUrl = $"https://pagespeedonline.googleapis.com/pagespeedonline/v5/runPagespeed" +
                $"?url={urlRequest}" +
                $"&category={_configuration.GetSection("PSI")["Category"]}" +
                $"&locale={_configuration.GetSection("PSI")["Locale"]}" +
                $"&key={_configuration.GetSection("PSI")["APIKey"]} " +
                $"&strategy={strategy}";

            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new NotFoundException($"Page {urlRequest} not found!");
            }

            return response.ReasonPhrase;
        }
        public async Task<PageSpeedReport> ReportCleanAsync(string reportRequest, Strategy strategy)
        {
            var report = await GenerateReportAsync(reportRequest, strategy);
            var pageSpeedReport = new PageSpeedReport();
            pageSpeedReport.PageSpeedElement = new List<PageSpeedElement>();

            if (report is null)
                throw new BadRequestException("Report is null");
            
            try
            {
                dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(report);
                
                if (jsonData.lighthouseResult.requestedUrl == null)
                    throw new BadRequestException($"Error report: {report}");

                pageSpeedReport.WebName = jsonData.lighthouseResult.requestedUrl;
                pageSpeedReport.Result = jsonData.lighthouseResult.categories.accessibility.score;
                pageSpeedReport.Strategy = strategy;
                pageSpeedReport.PSIVersion = jsonData.lighthouseResult.lighthouseVersion;
                pageSpeedReport.SystemVersion = jsonData.lighthouseResult.environment.networkUserAgent;                

                foreach (var audit in jsonData.lighthouseResult.audits)
                {
                    if (audit.Value.score == 0)
                    {
                        var pageSpeedElement = new PageSpeedElement();
                        pageSpeedElement.PageSpeedSubElement = new List<PageSpeedSubElement>();
                        pageSpeedElement.ElementName = audit.Value.id;
                        pageSpeedElement.Title = audit.Value.title;
                        pageSpeedElement.Description = audit.Value.description;
                        pageSpeedElement.TypeOfResult = TypeOfResult.ERROR;

                        
                        if (audit.Value.details.items.Count > 0)
                        {
                            foreach (var item in audit.Value.details.items)
                            {
                                var pageSpeedSubElement = new PageSpeedSubElement();
                                pageSpeedSubElement.Snippet = item.node.snippet;
                                pageSpeedSubElement.Selector = item.node.selector;

                                pageSpeedElement.PageSpeedSubElement.Add(pageSpeedSubElement);
                            }
                        }
                        pageSpeedReport.PageSpeedElement.Add(pageSpeedElement);
                    }
                    else if (audit.Value.score == 1)
                    {
                        var pageSpeedElement = new PageSpeedElement();
                        pageSpeedElement.PageSpeedSubElement = new List<PageSpeedSubElement>();
                        pageSpeedElement.ElementName = audit.Value.id;
                        pageSpeedElement.Title = audit.Value.title;
                        pageSpeedElement.Description = audit.Value.description;
                        pageSpeedElement.TypeOfResult = TypeOfResult.PASSED;

                        pageSpeedReport.PageSpeedElement.Add(pageSpeedElement);
                    }
                }
            }
            catch (Exception)
            {
                throw new BadRequestException($"Error report2: {report}");
            }

            //if (reportRequest.Save)
            //{
            //    var isSaved = await SaveReportAsync(pageSpeedReport);
            //    if (isSaved is false)
            //    {
            //        throw new BadRequestException($"Error with save Report: {pageSpeedReport.WebName}");
            //    }
            //}
            
            return pageSpeedReport;
        }
        public async Task<List<PageSpeedReport>> GetReportsAsync(PageSpeedRequest reportRequest)
        {
            
            var reports = new List<PageSpeedReport>();

            foreach (var url in reportRequest.Urls)
            {
                if (reportRequest.GenerateForExistsReport is true)
                {
                    //check if url exist in webPage Table
                    await _webPageService.CheckInDBAsync(url);
                    var guidWebLink = await _myDbContext.WebPages.FirstOrDefaultAsync(x => x.WebName.ToLower() == url.ToLower());
                    //generowanie raportu, czy istnieje raport w db czy nie
                    var report = await ReportCleanAsync(url, reportRequest.Strategy);
                    report.WebPageId = guidWebLink.Id;
                    reports.Add(report);
                }
                else
                {
                    var getReport = await _myDbContext.PageSpeedReports.FirstOrDefaultAsync(x => x.WebName.ToLower() == url.ToLower() && x.Strategy == reportRequest.Strategy);
                    if(getReport == null)
                    {
                        //check if url exist in webPage Table
                        await _webPageService.CheckInDBAsync(url);
                        var guidWebLink = await _myDbContext.WebPages.FirstOrDefaultAsync(x => x.WebName.ToLower() == url.ToLower());

                        var report = await ReportCleanAsync(url,reportRequest.Strategy);
                        report.WebPageId = guidWebLink.Id;
                        reports.Add(report);
                    }
                }
            }

            if (reportRequest.Save is true)
            {
                foreach (var report in reports)
                {
                    //Check if report exists in DB
                    var getReport = await _myDbContext.PageSpeedReports.FirstOrDefaultAsync(x => x.WebName.ToLower() == report.WebName.ToLower() && x.Strategy == reportRequest.Strategy);
                    if (getReport != null)
                    {
                        var resultDelete = await DeleteReportAsync(getReport.Id);
                    }
                
                    var isSaved = await SaveReportAsync(report);
                    if(isSaved == Guid.Empty)
                    {
                        throw new BadRequestException($"Error with save Report: {report.WebName}");
                    }
                }
            }

            return reports;
        }

        public async Task<List<PageSpeedReport>> GetReportsAsync(PageSpeedRequestDomain pageSpeedRequestDomain)
        {
            var reports = new List<PageSpeedReport>();
            var linksFromDB = await _myDbContext.WebPages.Where(x => x.DomainName.ToLower() == pageSpeedRequestDomain.Domain.ToLower()).ToListAsync();

            foreach (var url in linksFromDB)
            {
                if (pageSpeedRequestDomain.GenerateForExistsReport is true)
                {
                    //generowanie raportu, czy istnieje raport w db czy nie
                    var report = await ReportCleanAsync(url.WebName, pageSpeedRequestDomain.Strategy);
                    report.WebPageId = url.Id;
                    reports.Add(report);
                }
                else
                {
                    var getReport = await _myDbContext.PageSpeedReports.FirstOrDefaultAsync(x => x.WebName.ToLower() == url.WebName.ToLower() && x.Strategy == pageSpeedRequestDomain.Strategy);
                    if (getReport == null)
                    {
                        //check if url exist in webPage Table

                        var report = await ReportCleanAsync(url.WebName, pageSpeedRequestDomain.Strategy);
                        report.WebPageId = url.Id;
                        reports.Add(report);
                    }
                }
            }

            if (pageSpeedRequestDomain.Save is true)
            {
                foreach (var report in reports)
                {
                    //Check if report exists in DB
                    var getReport = await _myDbContext.PageSpeedReports.FirstOrDefaultAsync(x => x.WebName.ToLower() == report.WebName.ToLower() && x.Strategy == pageSpeedRequestDomain.Strategy);
                    if (getReport != null)
                    {
                        var resultDelete = await DeleteReportAsync(getReport.Id);
                    }

                    var isSaved = await SaveReportAsync(report);
                    if (isSaved == Guid.Empty)
                    {
                        throw new BadRequestException($"Error with save Report: {report.WebName}");
                    }
                }
            }

            return reports;
        }

        public async Task<Guid> SaveReportAsync(PageSpeedReport pageSpeedReport)
        {
            await _myDbContext.PageSpeedReports.AddAsync(pageSpeedReport);

            var result = _myDbContext.SaveChanges();
            if(result == 0)
               return Guid.Empty;

            return pageSpeedReport.Id;
        }

        public async Task<PageSpeedReport> GetReportFromDBAsync(Guid guid)
        {
            var result = await _myDbContext.PageSpeedReports
                .Include(x => x.PageSpeedElement)
                    .ThenInclude(y => y.PageSpeedSubElement)
                .FirstOrDefaultAsync(x => x.Id == guid);
            
            if(result == null)
            {
                throw new NotFoundException("No report with given id found");
            }
            return result;
        }


        public async Task<bool> DeleteReportAsync(Guid guid)
        {
            var element = await _myDbContext.PageSpeedReports
                .Include(x => x.PageSpeedElement)
                    .ThenInclude(y => y.PageSpeedSubElement)
                .FirstOrDefaultAsync(x => x.Id == guid);

            if (element == null)
                throw new NotFoundException("No report with given id found");

            //Delete From PageSpeedSubElements
            _myDbContext.PageSpeedSubElements.RemoveRange(element.PageSpeedElement.SelectMany(tb => tb.PageSpeedSubElement).ToList());

            //Delete From PageSpeedElements
            _myDbContext.PageSpeedElements.RemoveRange(element.PageSpeedElement);

            //Delete From PageSpeedReport
            _myDbContext.PageSpeedReports.Remove(element);

            //Save Changes
            var result = await _myDbContext.SaveChangesAsync();

            if (result > 0)
                return true;

            return false;
        }
        public async Task<List<PageSpeedMultiReportResponse>> GetMultipleReport(string domain)
        {
            var psi = new List<PageSpeedMultiReportResponse>();
            var linksFromDB = await _myDbContext.WebPages
                .Where(x => x.DomainName == domain)
                .Select(x => x.Id)
                .ToListAsync();

            var reportsFromDB = await _myDbContext.PageSpeedReports
                .Include(x => x.PageSpeedElement)
                .Where(x => linksFromDB
                .Contains(x.WebPageId))
                .ToListAsync();
            
            foreach (var report in reportsFromDB)
            {
                var psiReport = new PageSpeedMultiReportResponse()
                {
                    Id = report.Id,
                    WebName = report.WebName,
                    DateTime = report.Date,
                    Result = report.Result,
                    Strategy = report.Strategy,
                    AmountOfErrors = report.PageSpeedElement.Where(x => x.TypeOfResult == TypeOfResult.ERROR).Count(),
                    AmountOfPassed = report.PageSpeedElement.Where(x => x.TypeOfResult == TypeOfResult.PASSED).Count()
                };
                psi.Add(psiReport);
            }
            return psi;
        }
    }
}
