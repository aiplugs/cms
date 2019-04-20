using Aiplugs.CMS.Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Aiplugs.CMS.Web.ViewModels
{
    public class JobViewModel
    {
        public int Limit { get; set; } = 20;
        public bool Desc { get; set; } = true;
        public IEnumerable<Job> Jobs { get; set; }
        public Job Job { get; set; }

        public bool HasPrev() =>  (Desc == false && Jobs.Count() == Limit) || Job != null;
        public bool HasNext() => Desc == true && Jobs.Count() == Limit;
        public string PrevSkipToken() => Job.Id;
        public string NextSkipToken() => Jobs.Last().Id;
    }
}
