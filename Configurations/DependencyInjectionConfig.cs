using System.Reflection;

namespace Configurations
{
    public class DependencyInjectionConfig
    {
        public static IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>();
            assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());

            assemblies.Add(Assembly.Load("UserSystem.Repositories"));
            assemblies.Add(Assembly.Load("UserSystem.Services"));

            return assemblies;
        }
    }
}
