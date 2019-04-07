namespace Aiplugs.CMS.Core.Models
{
    public class ContextParameters : IContextParameters
    {
        public string[] Items { get; set; }

        public SearchMethod SearchMethod { get; set;}

        public string SearchQuery { get; set; }

        public string CollectionName { get; set; }
    }
}