namespace Aiplugs.CMS.Query
{
    public class QResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public IQuery Expression { get; set; }
    }
}