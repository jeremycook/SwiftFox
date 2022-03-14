using SwiftFox.Services;
using SwiftFox.UI;

namespace SwiftFox.Pages.Shared.UI
{
    [Service(ServiceType.Scoped)]
    public class SiteLinks
    {
        public List<Link> Links { get; } = new()
        {
            new Link("Home", "~/"),
        };
    }
}
