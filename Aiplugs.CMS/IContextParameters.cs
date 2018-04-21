namespace Aiplugs.CMS
{
    public interface IContextParameters
    {
        string CollectionName { get; }
        long[] Items { get; }
        SearchQueryType SearchQueryType { get; }
        string SearchQuery { get; }
    }
}