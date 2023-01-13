using System.Globalization;
using AngleSharp.Dom;
using MtgParser.Context;
using MtgParser.Model;

using IAngleConfig = AngleSharp.IConfiguration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;


namespace MtgParser.ParseLogic;

/// <summary>
/// main prise parser logic.. html in, staff out
/// </summary>
public class PriceParser : BaseParser
{
    private const string PriceSelector = ".price-box-price";


    /// <summary>
    /// Получение цены для физической карты
    /// </summary>
    /// <param name="cardSet">ссылка на физическую карту. фактически, достаточно названия и аббревиатуры сета</param>
    /// <param name="doc">html для разбора</param>
    /// <returns>цена карты</returns>
    /// <exception cref="Exception">полученные исключение просто перебрасываются выше, с выводом в консоль</exception>
    public static Price GetPrice(CardSet cardSet, IDocument doc)
    {
        try
        {
            Price? result = GetParsedPrice(doc);
            if (result == null)
            {
                Console.WriteLine("Can't parse price data " + cardSet.Id );
                throw new Exception();
            }
            
            result.CardSet = cardSet;
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private static Price? GetParsedPrice(IDocument doc)
    {
        IElement? priceBox = doc.QuerySelector(PriceSelector);
        if (priceBox == null)
        {
            return null;
        }
        
        string allDigits = GetSubStringAfterChar(priceBox.InnerHtml, ';');
        
        const NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
        CultureInfo provider = new ("en-GB");
        
        if (decimal.TryParse(allDigits,style, provider, out decimal price))
        {
            return new Price() 
            {
                Value = price, 
                CreateDate = DateTime.Now
            };
        }

        return null;
    }
}
