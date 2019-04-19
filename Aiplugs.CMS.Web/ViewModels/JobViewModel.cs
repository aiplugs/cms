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

        public bool HasNext() => Jobs.Count() == Limit;
        public string PrevSkipToken() => Job.Id.Substring(0, Job.Id.Length - 1);
        public string NextSkipToken() => Jobs.Last().Id.Substring(0, Jobs.Last().Id.Length - 1);
    }
}
