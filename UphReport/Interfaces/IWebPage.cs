using System.Collections.Generic;
using System.Threading.Tasks;
using UphReport.Models.WebPage;

namespace UphReport.Interfaces
{
    public interface IWebPage
    {
        public Task<List<string>> SearchUrlsAsync(WebPageDto webPageDto);
        Task<int> SaveLinksAsync(WebPageRequest urls);
        Task<bool> DeleteLinkAsync(Guid guid);
        Task<bool> DeleteLinksAboutDomainAsync(string domain);
        Task<bool> CheckInDBAsync(string url);
    }
}
