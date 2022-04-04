namespace Swiftfox.UI
{
    public class Link
    {
        public Link(string text, string url)
        {
            Text = text;
            Url = url;
        }

        public string Text { get; }
        public string Url { get; }
    }
}
