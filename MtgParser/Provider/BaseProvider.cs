using System.Text;
using AngleSharp;
using AngleSharp.Dom;

namespace MtgParser.Provider;

public class BaseProvider
{
    protected static async Task<IDocument> GetHtmlAsync(string path)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        AngleSharp.IConfiguration config = Configuration.Default.WithDefaultLoader();
        IBrowsingContext context = BrowsingContext.New(config);
        return await context.OpenAsync(path);
    }
}