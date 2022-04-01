namespace SwiftFox.Pages.Shared.UI
{
    [Service(typeof(IValueMutator<SiteLinks>))]
    public class SwiftFoxSiteLinksMutator : IValueMutator<SiteLinks>
    {
        public ValueTask MutateAsync(SiteLinks siteLinks)
        {
            siteLinks.Links.Add(new("Home", "~/"));

            return ValueTask.CompletedTask;
        }
    }
}
