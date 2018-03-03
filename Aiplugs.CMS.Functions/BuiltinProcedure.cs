using System;
using System.Reflection;

namespace Aiplugs.CMS.Functions
{
  public class BuiltinProcedure : IProcedure
  {
    public string TypeName { get; }
    public string MethodName { get; }

    public BuiltinProcedure(string typeName, string methodName)
    {
      TypeName = typeName;
      MethodName = methodName;
    }
    public MethodInfo CreateMethod(Context context)
    {
      return Type.GetType(TypeName).GetMethod(MethodName, BindingFlags.Public | BindingFlags.Static);
    }
  }
}