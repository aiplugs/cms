using System.Reflection;

namespace Aiplugs.CMS.Functions
{
  public interface IProcedure
  {
    MethodInfo CreateMethod(Context context);
  }
}