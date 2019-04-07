using System.Collections.Generic;

namespace Aiplugs.CMS.Web.ViewModels
{
    public class CollectionViewModel
    {
        public SearchMethod SearchMethod { get; set; }
        public string SeachQuery { get; set; }
        public string TitlePath { get; set; }
        public string CollectionName { get; set; }
        public IEnumerable<IItem> Items { get; set; }
        public IEnumerable<ProcedureInfo> Procedures { get; set; }
        public string Prev { get; set; }
        public string Next { get; set; }
  }
}