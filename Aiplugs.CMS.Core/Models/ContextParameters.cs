namespace Aiplugs.CMS.Core.Models
{
    public class ContextParameters : IContextParameters
    {
        public long[] Items { get; set; }

        public SearchQueryType SearchQueryType { get; set;}
        public string SearchQuery { get; set; }

        public string CollectionName { get; set; }

    }
}