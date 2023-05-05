using System.Collections.Generic;
using System.Security.Policy;
using System;
using UphReport.Data;
using System.Linq;
using HtmlAgilityPack;
using UphReport.Interfaces;
using UphReport.Exceptions;
using UphReport.Models.WebPage;
using UphReport.Entities;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace UphReport.Services
{
    public class WebPageService : IWebPage
    {
        private readonly MyDbContext _myDbContext;
        public List<string> Urls { get; set; }

        public WebPageService(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }

        public async Task<bool> CheckInDBAsync(string url)
        {
            var ifExist = await _myDbContext.WebPages.FirstOrDefaultAsync(x => x.WebName.ToLower() == url.ToLower());

            if (ifExist != null)
            {
                return true;
            }
            try
            {
                WebClient client = new WebClient();
                string html = client.DownloadString(url);

                if (html.Contains("<html") && html.Contains("</html>"))
                {
                    WebPageRequest link = new WebPageRequest(new List<string>() { url });

                    var ifSaved = await SaveLinksAsync(link);
                    if (ifSaved == 0)
                    {
                        throw new BadRequestException("Link didnt save");
                    }
                    return true;
                }
                else
                {
                    throw new BadRequestException("URL is not a Web Page");
                }
            }
            catch (WebException)
            {
                throw new BadRequestException("Web Host not found");
            }
            

        }
        public async Task<List<string>> SearchUrlsAsync(WebPageDto webPageDto)
        {
            if (webPageDto.Depth < 1 || webPageDto.Depth > 5)
                throw new BadRequestException("Depth must be <1;5> ");

            var client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync(webPageDto.WebName);

                // Check if request was successful
                if (response.IsSuccessStatusCode)
                {
                    //Read content http as string
                    string html = await response.Content.ReadAsStringAsync();

                    // Parse HTML
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    // Check if doc contains element <html>
                    bool isWebsite = doc.DocumentNode.Descendants("html").Any();

                    if (isWebsite is false)
                    {
                        throw new BadRequestException($"{webPageDto.WebName} is not a website.");

                    }

                    Urls = new List<string>();
                    List<List<string>> urlsTemp = new List<List<string>>();
                    List<string> urlsEstimated = new List<string>();
                    urlsTemp.Add(new List<string>() { webPageDto.WebName });
                    string domain = GetBaseUrl(webPageDto.WebName);

                    for (int i = 0; i < webPageDto.Depth; i++)
                    {
                        Console.WriteLine($"Dzialam na poziomie {i + 1}");
                        urlsTemp.Add(new List<string>());
                        foreach (var item in urlsTemp.ElementAt(i))
                        {
                            var document = new HtmlWeb().Load(item);
                            var linkTags = document.DocumentNode.Descendants("link");
                            var linkedPages = document.DocumentNode.Descendants("a")
                                                                   .Select(a => a.GetAttributeValue("href", null))
                                                                   .Where(u => !String.IsNullOrEmpty(u));

                            urlsEstimated = Filter(linkedPages.ToList(), item, domain);
                            urlsEstimated = urlsEstimated.Except(urlsTemp[i + 1]).ToList();
                            urlsTemp[i + 1].AddRange(urlsEstimated);
                        }
                        Urls.AddRange(urlsTemp[i + 1]);
                    }
                    var webPageRequest = new WebPageRequest(Urls.Select(x => x).ToList());

                    //If Save?
                    if(webPageDto.SaveLinks)
                        await SaveLinksAsync(webPageRequest);

                    return Urls;
                }
                else
                {
                    throw new BadRequestException($"HTTP error: {response.StatusCode}");
                }
            }
            catch (Exception)
            {

                throw new BadRequestException($"{webPageDto.WebName} is not a website.");
            }
            
        }
        public async Task<int> SaveLinksAsync(WebPageRequest urls)
        {
            if (urls.Urls.Count == 0)
                throw new BadRequestException("Urls is empty");

            var domain = GetBaseUrl(urls.Urls[0]);

            var linksFromDB = _myDbContext.WebPages
                .Where(x => x.DomainName == domain)
                .Select(y => y.WebName)
                .ToList();

            var urlsToAdd = urls.Urls
                .Except(linksFromDB)
                .Select(x => new WebPage()
                    {
                        WebName = x,
                        DomainName = domain
                    })
                .ToList();

            await _myDbContext.WebPages.AddRangeAsync(urlsToAdd);
            var result = _myDbContext.SaveChanges();

            return result;
        }
        public async Task<bool> DeleteLinkAsync(Guid guid)
        {
            var result = await _myDbContext.WebPages.FirstOrDefaultAsync(x => x.Id == guid);
            if (result == null)
                throw new NotFoundException("Link with given id not found");

            _myDbContext.WebPages.Remove(result);
            await _myDbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> DeleteLinksAboutDomainAsync(string domain)
        {
            var findLinks = _myDbContext.WebPages.Where(x => x.DomainName == domain).ToList();
            if(findLinks.Count == 0)
            {
                throw new NotFoundException("Links with given domain name not found");
            }
            _myDbContext.RemoveRange(findLinks);
            await _myDbContext.SaveChangesAsync();

            return true;
        }

        private List<string> Filter(List<string> currentLinks, string currentPage, string domain)
        {
            //HasAbsoluteUrl
            for (int i = 0; i < currentLinks.Count; i++)
            {
                var uri = new Uri(currentLinks[i], UriKind.RelativeOrAbsolute);
                if (!uri.IsAbsoluteUri)
                    currentLinks[i] = new Uri(new Uri(currentPage), uri).ToString();
            }

            currentLinks = RejectOthersDomains(currentLinks, domain);
            currentLinks = RejectOtherExtension(currentLinks);
            return currentLinks;
        }
        private string GetBaseUrl(string url)
        {
            Uri uri = new Uri(url);
            string baseUrl = uri.Host;
            return baseUrl;
        }
        private List<string> RejectOthersDomains(List<string> currentLinks, string domain)
        {
            domain = domain.ToLower().Trim();
            currentLinks = currentLinks
                            .Where(x => GetBaseUrl(x.ToLower()) == domain
                                    && !x.Contains("/?")
                                    && !x.EndsWith("#")
                                    //&& !notAllowedExtensions.Any(ext => x.ToLower().EndsWith(ext))
                                    )
                            .Distinct()
                            .ToList();
            return currentLinks;
        }
        private List<string> RejectOtherExtension(List<string> currentLinks)
        {
            currentLinks = currentLinks
                            .Where(
                                link => link.EndsWith(".html")
                                    || link.EndsWith(".htm")
                                    || link.EndsWith(".php")
                                    || (link.LastIndexOf(".") < link.LastIndexOf("/") + 1 && link.Contains("."))
                                )
                            .Except(Urls)
                            .ToList();
            return currentLinks;
        }

    }
}
