namespace Aiplugs.CMS
{
    public interface IContextParameters
    {
        string CollectionName { get; }
        string[] Items { get; }
        SearchMethod SearchMethod { get; }
        string SearchQuery { get; }
    }
}