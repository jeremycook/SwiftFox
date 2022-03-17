namespace SwiftFox.Services
{
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
