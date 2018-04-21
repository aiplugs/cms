using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Aiplugs.Functions.Core;

namespace Aiplugs.CMS.Core.Models
{
    public class CustomProcedure : Aiplugs.Functions.Core.IProcedure
    {
        public string Path { get; }
        public string TypeName { get; }

        public CustomProcedure(string path, string typeName)
        {
            Path = path;
            TypeName = typeName;
        }
        public void Execute(IContext context)
        {
            var lib = Assembly.GetAssembly(typeof(Aiplugs.CMS.IContext)).GetName();
            var asm = Assembly.LoadFrom(Path);
            var refrence = asm.GetReferencedAssemblies().Where(a => a.Name == lib.Name).FirstOrDefault();

            if (refrence == null)
                throw new InvalidOperationException($"'{TypeName}' doesn't have dependence to Context.");

            if (refrence.FullName != lib.FullName)
                throw new InvalidOperationException($"Context versions are not match.");

            var type = Type.GetType($"{TypeName}, {asm.FullName}");

            var instance = Activator.CreateInstance(type) as IProcedure;
            
            if (instance == null)
                throw new ArgumentException(nameof(Path), "The type is not implemented Aiplugs.CMS.IProcedure");
            
            instance.Execute(context).Wait(context.CancellationToken);
        }
        public MethodInfo CreateMethod()
        {
            return this.GetType().GetMethod(nameof(Execute));
        }
    }
}