using System.Collections.Generic;
using System.Linq;

namespace Aiplugs.CMS.Web.ViewModels
{
    public class CollectionViewModel
    {
        public SearchMethod SearchMethod { get; set; }
        public string SeachQuery { get; set; }
        public string TitlePath { get; set; }
        public string CollectionName { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<IItem> Items { get; set; }
        public IEnumerable<ProcedureInfo> Procedures { get; set; }
        public string Prev { get; set; }
        public string Next { get; set; }
        public DataViewModel Data { get; set; }
        public int Limit { get; set; }
        public bool Desc { get; set; } = true;

        public bool HasPrev() => ( Desc == false && Items.Count() == Limit) || Data != null;
        public bool HasNext() => Desc == true && Items.Count() == Limit;
        public string PrevSkipToken() => Data.Item.Id;
        public string NextSkipToken() => Items.Last().Id;
    }
}