using System.Collections.Generic;
using System.Threading.Tasks;
using UphReport.Models.WebPage;

namespace UphReport.Interfaces
{
    public interface IWebPage
    {
        Task<List<string>> SearchUrlsAsync(WebPageDto webPageDto);
        Task<List<WebAllPageRequest>> GetAllUrlsAsync();
        Task<List<WebAllPageRequest>> GetAllUrlsAsync(string domainName);
        Task<int> SaveLinksAsync(WebPageRequest urls);
        Task<bool> DeleteLinkAsync(Guid guid);
        Task<bool> DeleteLinksAboutDomainAsync(string domain);
        Task<bool> CheckInDBAsync(string url);
        Task<List<string>> GetAllDomainAsync();
        int GetAmountWebAboutDomain(string domainName);
    }
}
