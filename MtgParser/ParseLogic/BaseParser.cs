namespace MtgParser.ParseLogic;

/// <summary>
/// common for all parsers logic
/// </summary>
public abstract class BaseParser
{
    /// <summary>
    /// use it to parse mana cost
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    protected static bool IsDigitOrX(char symbol)
    {
        return char.IsDigit(symbol) || symbol == 'X';
    }
    
    /// <summary>
    /// intricate method.. separate english and russian text, type etc
    /// </summary>
    /// <param name="source">string to separate</param>
    /// <param name="separator">separator // - default</param>
    /// <returns>left and right parts</returns>
    /// <exception cref="ArgumentException">if source is null</exception>
    protected static (string main, string substr) GetSeparateString(string? source, string separator = "//")
    {
        if (string.IsNullOrEmpty(source))
        {
            throw new ArgumentException("source can't be null");
        }
        
        int separatorIndex = source.IndexOf(separator, StringComparison.Ordinal);
        if (separatorIndex < 0)
        {
            return (source.Trim('\r', '\n', '\t', ' '), string.Empty);
        }
        
        string left = source[..separatorIndex].Trim('\r', '\n', '\t', ' ');
        string right = source[separatorIndex..].Trim('\r', '\n','\t', '/', ' ');
        return left != right ? (left, right) : (left, string.Empty);
    }
    
    /// <summary>
    /// to check numerous split conditions in one pass 
    /// </summary>
    /// <param name="text">string source</param>
    /// <param name="separators">chars to split string</param>
    /// <returns>substring after first of separators</returns>
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