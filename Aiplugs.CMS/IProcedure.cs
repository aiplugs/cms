using System.Threading.Tasks;

namespace Aiplugs.CMS
{
    public interface IProcedure
    {
        Task Execute(IContext context);
    }
}