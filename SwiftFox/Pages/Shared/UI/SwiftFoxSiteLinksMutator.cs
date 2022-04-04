namespace Swiftfox.Pages.Shared.UI
{
    [Service(typeof(IValueMutator<SiteLinks>))]
    public class SwiftfoxSiteLinksMutator : IValueMutator<SiteLinks>
    {
        public ValueTask MutateAsync(SiteLinks siteLinks)
        {
            siteLinks.Links.Add(new("Home", "~/"));

            return ValueTask.CompletedTask;
        }
    }
}
