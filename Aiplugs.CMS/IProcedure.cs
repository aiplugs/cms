using System.Threading.Tasks;

namespace Aiplugs.CMS
{
    public interface IProcedure
    {
        Task ExecuteAsync(IContext context);
    }
}