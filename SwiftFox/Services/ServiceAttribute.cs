namespace SwiftFox.Services
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(ServiceType serviceScope)
        {
            ServiceScope = serviceScope;
        }

        public ServiceAttribute(ServiceType serviceScope, Type serviceType)
        {
            ServiceScope = serviceScope;
            ServiceType = serviceType;
        }

        public ServiceType ServiceScope { get; }
        public Type? ServiceType { get; }
    }
}
