namespace UphReport.Models.WebPage
{
    public class WebPageDto
    {
        public string WebName { get; set; }
        public int Depth { get; set; }
        public bool SaveLinks { get; set; }

        public WebPageDto()
        {

        }
    }
}
