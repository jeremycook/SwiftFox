namespace SwiftFox.Pages.Shared.UI
{
    [Service(typeof(IValueMutator<SiteLinks>))]
    public class DefaultSiteLinksContributor : IValueMutator<SiteLinks>
    {
        public ValueTask MutateAsync(SiteLinks siteLinks)
        {
            siteLinks.Links.Add(new("Home", "~/"));

            return ValueTask.CompletedTask;
        }
    }
}
