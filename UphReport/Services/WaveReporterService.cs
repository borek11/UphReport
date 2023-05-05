using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UphReport.Data;
using UphReport.Entities.PageSpeedInsights;
using UphReport.Entities.Wave;
using UphReport.Exceptions;
using UphReport.Interfaces;
using UphReport.Models.PSI;
using UphReport.Models.Wave;

namespace UphReport.Services;

public class WaveReporterService : IWaveReporterService
{
	private readonly MyDbContext _myDbContext;
	private readonly HttpClient _httpClient;
	private readonly IWaveAPIKeyService _waveAPIKeyService;
	private readonly IWebPage _webPageService;

	public WaveReporterService(MyDbContext myDbContext, HttpClient httpClient, IWaveAPIKeyService waveAPIKeyService, IWebPage webPageService)
	{
		_myDbContext = myDbContext;
		_httpClient = httpClient;
		_waveAPIKeyService = waveAPIKeyService;
		_webPageService = webPageService;
	}

	public async Task<string> GenerateReportAsync(WaveReportRequest waveReportRequest)
	{
		var requestUrl = $"https://wave.webaim.org/api/request" +
			$"?key={waveReportRequest.Key}" +
			$"&reporttype=4" +
			$"&url={waveReportRequest.Url}";

		var response = await _httpClient.GetAsync(requestUrl);

		var result = await response.Content.ReadAsStringAsync();

        dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(result);
		
		if(jsonData.status.success == false)
		{
			return null;
		}
        return result;
    }
	public async Task<WaveReport> ReportCleanAsync(string reportRequest, Strategy strategy)
	{
		var waveReportRequest = new WaveReportRequest()
		{
			Url = reportRequest,
			Key = await _waveAPIKeyService.GetAPIKey(),
			Strategy = strategy
		};

		var report = await GenerateReportAsync(waveReportRequest);
		var waveReport = new WaveReport();

		if(report is null){
            throw new BadRequestException("Report is null");
        }
		try
		{
            dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(report);
			if(jsonData == null) {
				throw new BadRequestException("Report is null");
			}

			waveReport.WebName = jsonData.statistics.pageurl;
			waveReport.Strategy = Strategy.DESKTOP;
			waveReport.WaveElements = new List<WaveElement>();

			await _waveAPIKeyService.UpdateKey(new WaveAKUpdate()
			{
				APIKey = waveReportRequest.Key,
				CreditsRemaining = jsonData.statistics.creditsremaining
			});

			foreach (var audit in jsonData.categories)
			{
				if(audit.Name == "error" || audit.Name == "contrast")
				{
					foreach (var item in audit.Value.items)
					{
						var waveElement = new WaveElement
						{
							WaveSubElements = new List<WaveSubElement>(),
							ElementName = item.Value.id,
							Description = item.Value.description,
							TypeOfResult = TypeOfResult.ERROR
						};
						int count = item.Value.count;
                        if (count > 0)
						{
							for (int i = 0; i < count; i++)
							{
								var waveSubElement = new WaveSubElement
								{
									Selector = item.Value.selectors[i]
								};
								waveElement.WaveSubElements.Add(waveSubElement);
                            }
                        }
						waveReport.WaveElements.Add(waveElement);
                    }
                }
				else if(audit.Name == "alert")
				{
                    foreach (var item in audit.Value.items)
                    {
						var waveElement = new WaveElement
						{
                            WaveSubElements = new List<WaveSubElement>(),
                            ElementName = item.Value.id,
							Description = item.Value.description,
							TypeOfResult = TypeOfResult.WARNING
						};
						int count = item.Value.count;
                        if (count > 0)
                        {
                            for (int i = 0; i < count; i++)
                            {
								var waveSubElement = new WaveSubElement
								{
									Selector = item.Value.selectors[i]
								};
								waveElement.WaveSubElements.Add(waveSubElement);
                            }
                        }
                        waveReport.WaveElements.Add(waveElement);
                    }
                }
				else if(audit.Name == "feature")
				{
                    foreach (var item in audit.Value.items)
                    {
						var waveElement = new WaveElement
						{
                            WaveSubElements = new List<WaveSubElement>(),
                            ElementName = item.Value.id,
							Description = item.Value.description,
							TypeOfResult = TypeOfResult.PASSED
						};
						int count = item.Value.count;
                        if (count > 0)
                        {
                            for (int i = 0; i < count; i++)
                            {
								var waveSubElement = new WaveSubElement
								{
									Selector = item.Value.selectors[i]
								};
								waveElement.WaveSubElements.Add(waveSubElement);
                            }
                        }
                        waveReport.WaveElements.Add(waveElement);
                    }
                }
				//else
				//{

				//}
			}

        }
		catch (Exception)
		{

            throw new BadRequestException($"Error report2: {report}");
        }

		return waveReport;
	}
	public async Task<List<WaveReport>> GetReportsAsync(WaveRequests waveRequests)
	{
		var reports = new List<WaveReport>();

		foreach (var url in waveRequests.Urls)
		{
			if(waveRequests.GenerateForExistsReport is true)
			{
                //check if url exist in webPage Table
                await _webPageService.CheckInDBAsync(url);
                var guidWebLink = await _myDbContext.WebPages.FirstOrDefaultAsync(x => x.WebName.ToLower() == url.ToLower());
                //generowanie raportu, czy istnieje raport w db czy nie
                var report = await ReportCleanAsync(url, waveRequests.Strategy);
                report.WebPageId = guidWebLink.Id;
                reports.Add(report);
            }
			else
			{
                var getReport = await _myDbContext.PageSpeedReports.FirstOrDefaultAsync(x => x.WebName.ToLower() == url.ToLower() && x.Strategy == waveRequests.Strategy);
                if (getReport == null)
                {
                    //check if url exist in webPage Table
                    await _webPageService.CheckInDBAsync(url);
                    var guidWebLink = await _myDbContext.WebPages.FirstOrDefaultAsync(x => x.WebName.ToLower() == url.ToLower());

                    var report = await ReportCleanAsync(url, waveRequests.Strategy);
                    report.WebPageId = guidWebLink.Id;
                    reports.Add(report);
                }
            }
		}

        if (waveRequests.Save is true)
        {
            foreach (var report in reports)
            {
                //Check if report exists in DB
                var getReport = await _myDbContext.WaveReports.FirstOrDefaultAsync(x => x.WebName.ToLower() == report.WebName.ToLower() && x.Strategy == waveRequests.Strategy);
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
    public async Task<List<WaveReport>> GetReportsAsync(WaveDomainRequests waveDomainRequests)
    {
        var reports = new List<WaveReport>();
        var linksFromDB = await _myDbContext.WebPages.Where(x => x.DomainName.ToLower() == waveDomainRequests.Domain.ToLower()).ToListAsync();

        foreach (var url in linksFromDB)
        {
            if (waveDomainRequests.GenerateForExistsReport is true)
            {
                //generowanie raportu, czy istnieje raport w db czy nie
                var report = await ReportCleanAsync(url.WebName, waveDomainRequests.Strategy);
                report.WebPageId = url.Id;
                reports.Add(report);
            }
            else
            {
                var getReport = await _myDbContext.WaveReports.FirstOrDefaultAsync(x => x.WebName.ToLower() == url.WebName.ToLower() && x.Strategy == waveDomainRequests.Strategy);
                if (getReport == null)
                {
                    //check if url exist in webPage Table

                    var report = await ReportCleanAsync(url.WebName, waveDomainRequests.Strategy);
                    report.WebPageId = url.Id;
                    reports.Add(report);
                }
            }
        }

        if (waveDomainRequests.Save is true)
        {
            foreach (var report in reports)
            {
                //Check if report exists in DB
                var getReport = await _myDbContext.WaveReports.FirstOrDefaultAsync(x => x.WebName.ToLower() == report.WebName.ToLower() && x.Strategy == waveDomainRequests.Strategy);
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
    public async Task<Guid> SaveReportAsync(WaveReport waveReport)
	{
        await _myDbContext.WaveReports.AddAsync(waveReport);

        var result = _myDbContext.SaveChanges();
        if (result == 0)
            return Guid.Empty;

        return waveReport.Id;
    }
    public async Task<WaveReport> GetReportFromDBAsync(Guid guid)
    {
        var result = await _myDbContext.WaveReports
            .Include(x => x.WaveElements)
                .ThenInclude(y => y.WaveSubElements)
            .FirstOrDefaultAsync(x => x.Id == guid);

        if (result == null)
        {
            throw new NotFoundException("No report with given id found");
        }
        return result;
    }


    public async Task<bool> DeleteReportAsync(Guid guid)
    {
        var element = await _myDbContext.WaveReports
            .Include(x => x.WaveElements)
                .ThenInclude(y => y.WaveSubElements)
            .FirstOrDefaultAsync(x => x.Id == guid);

        if (element == null)
            throw new NotFoundException("No report with given id found");

        //Delete From PageSpeedSubElements
        _myDbContext.WaveSubElements.RemoveRange(element.WaveElements.SelectMany(tb => tb.WaveSubElements).ToList());

        //Delete From PageSpeedElements
        _myDbContext.WaveElements.RemoveRange(element.WaveElements);

        //Delete From PageSpeedReport
        _myDbContext.WaveReports.Remove(element);

        //Save Changes
        var result = await _myDbContext.SaveChangesAsync();

        if (result > 0)
            return true;

        return false;
    }
    public async Task<List<WaveMultiReportResponse>> GetMultipleReport(string domain)
    {
        var wave = new List<WaveMultiReportResponse>();
        var linksFromDB = await _myDbContext.WebPages
            .Where(x => x.DomainName == domain)
            .Select(x => x.Id)
            .ToListAsync();

        var reportsFromDB = await _myDbContext.WaveReports
            .Include(x => x.WaveElements)
            .Where(x => linksFromDB
            .Contains(x.WebPageId))
            .ToListAsync();

        foreach (var report in reportsFromDB)
        {
            var waveReport = new WaveMultiReportResponse()
            {
                Id = report.Id,
                WebName = report.WebName,
                DateTime = report.Date,
                Strategy = report.Strategy,
                AmountOfErrors = report.WaveElements.Where(x => x.TypeOfResult == TypeOfResult.ERROR).Count(),
                AmountOfPassed = report.WaveElements.Where(x => x.TypeOfResult == TypeOfResult.PASSED).Count()
            };
            wave.Add(waveReport);
        }
        return wave;
    }
}
