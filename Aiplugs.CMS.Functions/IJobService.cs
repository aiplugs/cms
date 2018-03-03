using System;
using System.Collections.Generic;

namespace Aiplugs.CMS.Functions
{
  public interface IJobService
  {
      
      Job Dequeue();
      void Update(Job job);

      void RegisterCanceller(Job job, Action canceller);
      void UnregisterCanceller(Job job);
      void CancelAll();
  }
}