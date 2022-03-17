using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftFox.Services;
using System.Reflection;

namespace SwiftFox.Startup
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures any type with a name that ends with "Options" using the
        /// section matching the name of the option without the "Options" suffix.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <param name="configuration"></param>
        public static void ConfigureOptionsFromAssemblies(this IServiceCollection services, IEnumerable<Assembly> assemblies, IConfiguration configuration)
        {
            string options = "Options";
            MethodInfo Configure = typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethod("Configure", 1, BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(IServiceCollection), typeof(IConfiguration) }, null)!;

            IEnumerable<Type> optionTypes = assemblies
                .SelectMany(a => a.ExportedTypes)
                .Where(t => t.Name.EndsWith(options));

            foreach (var type in optionTypes)
            {
                MethodInfo? Configure_T = Configure.MakeGenericMethod(type);
                string sectionName = type.Name[..^options.Length];
                IConfigurationSection? section = configuration.GetSection(sectionName);

                // Equivalent to: services.Configure<SwiftFoxOptions>(configuration.GetSection("SwiftFox"));
                Configure_T.Invoke(null, new object?[] { services, section });
            }
        }

        /// <summary>
        /// Adds types with the <see cref="ServiceAttribute"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <exception cref="NotSupportedException"></exception>
        public static void AddServicesFromAssemblies(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            foreach (var implementationType in assemblies.SelectMany(a => a.ExportedTypes))
            {
                if (implementationType.GetCustomAttribute<ServiceAttribute>() is ServiceAttribute attribute)
                {
                    switch (attribute.ServiceLifetime)
                    {
                        case Services.ServiceLifetime.Scoped:
                            if (attribute.ServiceType is not null)
                                services.AddScoped(attribute.ServiceType, implementationType);
                            else
                                services.AddScoped(implementationType);
                            break;

                        case Services.ServiceLifetime.Singleton:
                            if (attribute.ServiceType is not null)
                                services.AddSingleton(attribute.ServiceType, implementationType);
                            else
                                services.AddSingleton(implementationType);
                            break;

                        case Services.ServiceLifetime.Transient:
                            if (attribute.ServiceType is not null)
                                services.AddTransient(attribute.ServiceType, implementationType);
                            else
                                services.AddTransient(implementationType);
                            break;

                        default:
                            throw new NotSupportedException($"The '{attribute.ServiceType}' service type is not supported.");
                    }
                }
            }
        }
    }
}
