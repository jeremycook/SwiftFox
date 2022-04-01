using SwiftFox.Configuration;

namespace SwiftFox
{
    [Options]
    public class SwiftFoxOptions
    {
        public string AppTitle { get; set; } = "Configuration=SwiftFox:AppTitle";
        public string MainConnectionString { get; set; } = "Configuration=SwiftFox:MainConnectionString";
    }
}
