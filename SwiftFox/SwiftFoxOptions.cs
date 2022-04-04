using Swiftfox.Configuration;

namespace Swiftfox
{
    [Options]
    public class SwiftfoxOptions
    {
        public string AppTitle { get; set; } = "Configuration=Swiftfox:AppTitle";
        public string MainConnectionString { get; set; } = "Configuration=Swiftfox:MainConnectionString";
    }
}
