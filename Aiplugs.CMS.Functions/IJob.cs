using System;

namespace Aiplugs.CMS.Functions
{
  public interface IJob
  {
    string Name { get; set; }
    JobStatus Status { get; set; }
    DateTimeOffset? Start { get; set; }
    DateTimeOffset? Finish { get; set; }
    string Log { get; set; }
    int Progress { get; set; }
  }
}