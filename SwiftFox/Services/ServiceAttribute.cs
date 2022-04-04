namespace Swiftfox.Services
{
    /// <summary>
    /// Mark a class as a service. The <see cref="ServiceLifetime"/> defaults to <see cref="ServiceLifetime.Scoped"/>.
    /// </summary>
    /// <param name="serviceLifetime"></param>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            ServiceLifetime = serviceLifetime;
        }

        public ServiceAttribute(Type serviceType, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            ServiceType = serviceType;
            ServiceLifetime = serviceLifetime;
        }

        public ServiceLifetime ServiceLifetime { get; }
        public Type? ServiceType { get; }
    }
}
