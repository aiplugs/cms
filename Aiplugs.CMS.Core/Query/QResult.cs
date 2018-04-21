namespace Aiplugs.CMS.Core.Query
{
    public class QResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public IQuery Expression { get; set; }
    }
}