using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UphReport.Data;
using UphReport.Entities.PageSpeedInsights;
using UphReport.Exceptions;
using UphReport.Interfaces;
using UphReport.Models.PSI;
using UphReport.Models.User;

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
        public async Task<List<PageSpeedReport>> GetReportsAsync(PageSpeedRequest reportRequest, string token)
        {
            var userId = await GetUserId(token);

            var reports = new List<PageSpeedReport>();

            var webId = new Guid[reportRequest.Urls.Count];
            int counter = 0;

            foreach (var url in reportRequest.Urls)
            {
                if (reportRequest.GenerateForExistsReport is true)
                {
                    //check if url exist in webPage Table
                    await _webPageService.CheckInDBAsync(url);
                        //var guidWebLink = await _myDbContext.WebPages.FirstOrDefaultAsync(x => x.WebName.ToLower() == url.ToLower());
                    var guidWebLink = await _myDbContext.WebPages.FirstOrDefaultAsync(x => x.WebName.ToLower() == url.ToLower() && x.DomainName == reportRequest.DomainName);
                    
                    if (guidWebLink != null)
                        webId[counter] = guidWebLink.Id;
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
                            //var guidWebLink = await _myDbContext.WebPages.FirstOrDefaultAsync(x => x.WebName.ToLower() == url.ToLower());
                        var guidWebLink = await _myDbContext.WebPages.FirstOrDefaultAsync(x => x.WebName.ToLower() == url.ToLower() && x.DomainName == reportRequest.DomainName);

                        if (guidWebLink != null)
                            webId[counter] = guidWebLink.Id;

                        var report = await ReportCleanAsync(url,reportRequest.Strategy);
                        report.WebPageId = guidWebLink.Id;
                        reports.Add(report);
                    }
                }
                counter++;
            }

            counter = 0;

            if (reportRequest.Save is true)
            {
                foreach (var report in reports)
                {
                    //Check if report exists in DB
                        //var getReport = await _myDbContext.PageSpeedReports.FirstOrDefaultAsync(x => x.WebName.ToLower() == report.WebName.ToLower() && x.Strategy == reportRequest.Strategy);
                    var getReport = await _myDbContext.PageSpeedReports.FirstOrDefaultAsync(x => x.WebPageId == webId[counter] && x.Strategy == reportRequest.Strategy);
                    if (getReport != null)
                    {
                        var resultDelete = await DeleteReportAsync(getReport.Id);
                    }
                    report.CreatedById = userId;
                    var isSaved = await SaveReportAsync(report);
                    if(isSaved == Guid.Empty)
                    {
                        throw new BadRequestException($"Error with save Report: {report.WebName}");
                    }

                    counter++;
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
                    //var getReport = await _myDbContext.PageSpeedReports.FirstOrDefaultAsync(x => x.WebName.ToLower() == url.WebName.ToLower() && x.Strategy == pageSpeedRequestDomain.Strategy);
                    var getReport = await _myDbContext.PageSpeedReports.FirstOrDefaultAsync(x => x.WebPageId == url.Id && x.Strategy == pageSpeedRequestDomain.Strategy);
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

        public async Task<int> SaveMultiReport(List<PageSpeedReport> pageSpeedReports)
        {
            int counter = 0;
            foreach (var report in pageSpeedReports)
            {
                //Check if report exists in DB
                var getReport = await _myDbContext.PageSpeedReports.FirstOrDefaultAsync(x => x.WebName.ToLower() == report.WebName.ToLower() && x.Strategy == report.Strategy);
                if (getReport != null)
                {
                    var resultDelete = await DeleteReportAsync(getReport.Id);
                }

                var isSaved = await SaveReportAsync(report);
                if (isSaved == Guid.Empty)
                {
                    throw new BadRequestException($"Error with save Report: {report.WebName}");
                }
                counter++;
            }
            return counter;
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
        public async Task<List<PageSpeedMultiReportResponse>> GetOneReport(Guid webLinksId)
        {
            var psi = new List<PageSpeedMultiReportResponse>();

            var result = await _myDbContext.PageSpeedReports
                .Include(x => x.PageSpeedElement)
                .Where(x => x.WebPageId == webLinksId).ToListAsync();

            foreach (var report in result)
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
                .Where(x => linksFromDB.Contains(x.WebPageId))
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
        public async Task<List<PageSpeedMultiReportResponse>> GetMultipleReportByUser(int userId)
        {
            var psi = new List<PageSpeedMultiReportResponse>();
            var response = await _myDbContext.PageSpeedReports
                .Include(x => x.PageSpeedElement)
                .Where(x => x.CreatedById == userId)
                .ToListAsync();

            foreach (var report in response)
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


        public async Task<List<PageSpeedAndWebLinks>> GetLinksAndReportAsync(string domainName, int strategy)
        {
            Strategy strategyFromRequest = new Strategy();
            if (strategy == 0)
                strategyFromRequest = Strategy.DESKTOP;
            else if(strategy == 1)
                strategyFromRequest = Strategy.MOBILE;

            var linksFromDB = await _myDbContext.WebPages
             .Where(x => x.DomainName == domainName)
             .Select(x => new PageSpeedAndWebLinks()
             {
                 Id = x.Id,
                 WebName = x.WebName,
                 DomainName = x.DomainName
             })
             .ToListAsync();

            foreach (var item in linksFromDB)
            {
                var report = await _myDbContext.PageSpeedReports.FirstOrDefaultAsync(x => x.WebPageId == item.Id && x.Strategy == strategyFromRequest);
                if(report != null)
                {
                    item.ReportId = report.Id;
                    item.Result = report.Result;
                    item.Strategy = report.Strategy;
                    item.DateTime = report.Date;
                }
            }
            return linksFromDB;
        }

        public async Task<string> GetDomainByReportId(Guid reportId)
        {
            var webPageId = await _myDbContext.PageSpeedReports.FirstOrDefaultAsync(x => x.Id == reportId);
            if(webPageId == null)
            {
                throw new NotFoundException($"Nie znaleziono Raportu!");
            }
            var result = await _myDbContext.WebPages.FirstOrDefaultAsync(x => x.Id == webPageId.WebPageId);

            if (result == null)
            {
                throw new NotFoundException($"Nie znaleziono Raportu!");
            }

            return result.DomainName;
        }

        private async Task<int> GetUserId(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var userId = jwtSecurityToken.Claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

            var user = await _myDbContext.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (user == null)
            {
                throw new NotFoundException($"Nie znaleziono użytkownika!");
            }
            return user.Id;
        }

    }
}
