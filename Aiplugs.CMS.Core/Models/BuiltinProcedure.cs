using System;
using System.Reflection;

namespace Aiplugs.CMS.Core.Models
{
    public class BuiltinProcedure : Aiplugs.Functions.Core.IProcedure
    {
        public Type Type { get; protected set;}
        public BuiltinProcedure(Type type)
        {
            Type = type;
        }
        public void Execute(IContext context)
        {
            var instance = Activator.CreateInstance(Type) as IProcedure;
            
            if (instance == null)
                throw new ArgumentException(nameof(Type), "The type is not implemented Aiplugs.CMS.IProcedure");
            
            instance.ExecuteAsync(context).Wait(context.CancellationToken);
        }
        public MethodInfo CreateMethod()
        {
            return this.GetType().GetMethod(nameof(Execute));
        }
    }
}