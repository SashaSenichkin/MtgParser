namespace MtgParser.ParseLogic;

public abstract class BaseParser
{
    protected static bool IsDigitOrX(char symbol)
    {
        return char.IsDigit(symbol) || symbol == 'X';
    }
    
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
        
        string left = source[..separatorIndex].Trim('\r', '\n', ' ');
        string right = source[separatorIndex..].Trim('\r', '\n', '/', ' ');
        return left != right ? (left, right) : (left, string.Empty);
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