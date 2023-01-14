using System.Text;
using AngleSharp;
using AngleSharp.Dom;

namespace MtgParser.Provider;

/// <summary>
/// common for all providers logic
/// </summary>
public class BaseProvider
{
    /// <summary>
    /// ctor. some AngleSharp adjustments
    /// </summary>
    protected BaseProvider()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    /// <summary>
    /// download html by url. not a big deal
    /// </summary>
    /// <param name="url">download url</param>
    /// <returns>AngleSharp entity.. send it to parsers)</returns>
    protected static async Task<IDocument> GetHtmlAsync(string url)
    {
        AngleSharp.IConfiguration config = Configuration.Default.WithDefaultLoader();
        IBrowsingContext context = BrowsingContext.New(config);
        return await context.OpenAsync(url);
    }
}