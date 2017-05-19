using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Turbocharged.NSQ.Tests
{
#if FALSE
    public class MiscellaneousFacts
    {
        [Fact]
        public void EnsureNoAsyncVoidTests()
        {
            AssertNoAsyncVoidMethods(GetType().Assembly);
        }

        static void AssertNoAsyncVoidMethods(Assembly assembly)
        {
            var messages = assembly
                .GetAsyncVoidMethods()
                .Select(method =>
                    String.Format("'{0}.{1}' is an async void method.",
                        method.DeclaringType.Name,
                        method.Name))
                .ToList();
            Assert.False(messages.Any(),
                "Async void methods found!" + Environment.NewLine + String.Join(Environment.NewLine, messages));
        }
    }

    static class AssemblyExtensions
    {
        public static IEnumerable<MethodInfo> GetAsyncVoidMethods(this Assembly assembly)
        {
            return assembly.GetLoadableTypes()
              .SelectMany(type => type.GetMethods(
                BindingFlags.NonPublic
                | BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.Static
                | BindingFlags.DeclaredOnly))
              .Where(method => method.HasAttribute<System.Runtime.CompilerServices.AsyncStateMachineAttribute>())
              .Where(method => method.ReturnType == typeof(void));
        }

        static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        static bool HasAttribute<TAttribute>(this MethodInfo method) where TAttribute : Attribute
        {
            return method.GetCustomAttributes(typeof(TAttribute), false).Any();
        }
    }
#endif
}
