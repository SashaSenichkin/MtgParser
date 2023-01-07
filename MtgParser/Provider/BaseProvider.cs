using System.Text;
using AngleSharp;
using AngleSharp.Dom;

namespace MtgParser.Provider;

public class BaseProvider
{
    protected BaseProvider()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    protected static async Task<IDocument> GetHtmlAsync(string path)
    {
        AngleSharp.IConfiguration config = Configuration.Default.WithDefaultLoader();
        IBrowsingContext context = BrowsingContext.New(config);
        return await context.OpenAsync(path);
    }
}