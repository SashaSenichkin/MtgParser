using System.Text;
using AngleSharp;
using AngleSharp.Dom;

namespace MtgParser.ParseLogic;

public abstract class BaseParser
{
    protected static (string main, string substr) GetSeparateString(string? source, string separator = "//")
    {
        if (string.IsNullOrEmpty(source))
        {
            throw new ArgumentException("source can't be null");
        }
        
        int separatorIndex = source.IndexOf(separator, StringComparison.Ordinal);
        if (separatorIndex < 0)
        {
            return (source, string.Empty);
        }
        
        string left = source[..separatorIndex].Trim();
        string right = source[separatorIndex..].Trim('/', ' ');
        return left != right ? (left, right) : (left, string.Empty);
    }

    protected static async Task<IDocument> GetHtmlAsync(string path)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        IConfiguration config = Configuration.Default.WithDefaultLoader();
        IBrowsingContext context = BrowsingContext.New(config);
        return await context.OpenAsync(path);
    }

    protected static string GetSubStringAfterChar(string text, params char[] separators)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (separators.Contains(text[i]))
                return text[(i+1)..];
        }

        return text.Trim();
    }
}