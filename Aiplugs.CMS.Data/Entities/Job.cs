using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiplugs.CMS.Data.Entities
{
    public class Job
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public int Progress { get; set; }

        public JobStatus Status { get; set; }

        public DateTimeOffset? StartAt { get; set; }

        public DateTimeOffset? FinishAt { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string CreatedBy { get; set; }
        
        public string Log { get; set; }
        
        public string Parameters { get; set; }

        public Job()
        {
            Log = string.Empty;
            Progress = 0;
            Status = JobStatus.Ready;
            StartAt = null;
            FinishAt = null;
            CreatedBy = Guid.Empty.ToString();
        }
        public Job(string name, string createBy)
            : this(name, createBy, null)
        { }
        public Job(string name, string createBy, object @params)
            : this()
        {
            Name = name;
            CreatedAt = DateTimeOffset.UtcNow;
            CreatedBy = createBy;
            Parameters = JsonConvert.SerializeObject(@params);
        }
        public TParams GetParameters<TParams>()
        {
            return JsonConvert.DeserializeObject<TParams>(Parameters);
        }
    }
}
