namespace Aiplugs.CMS.Web.Services
{
  public interface IJobService : Aiplugs.CMS.Functions.IJobService
  {
    bool Lock(Procedure procedure);
    Job Find(long id);
    Job Create(Procedure procedure);
    void Enqueue(Job job);

    bool Cancel(Job job);
  }
}