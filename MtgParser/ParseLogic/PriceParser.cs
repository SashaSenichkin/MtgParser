using System.Globalization;
using AngleSharp.Dom;
using MtgParser.Model;

using IAngleConfig = AngleSharp.IConfiguration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;


namespace MtgParser.ParseLogic;

public class PriceParser : BaseParser
{
    private readonly IConfigurationSection _urlsConfig;

    public PriceParser(IConfiguration fullConfig)
    {
        _urlsConfig = fullConfig.GetSection("ExternalUrls");
    }

    /// <summary>
    /// Получение цены для физической карты
    /// </summary>
    /// <param name="cardSet">ссылка на физическую карту. фактически, достаточно названия и аббревиатуры сета</param>
    /// <returns>цена карты</returns>
    /// <exception cref="Exception">полученные исключение просто перебрасываются выше, с выводом в консоль</exception>
    public async Task<Price> GetPriceAsync(CardSet cardSet, IDocument doc)
    {
        try
        {
            string searchCardName = cardSet.Card.Name.Replace(' ', '+');
            //IDocument doc = await GetHtmlAsync($"{_urlsConfig[GoldfishPriceConfig] + cardSet.Set.SearchText}/{searchCardName}" );
 
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
        IElement? priceBox = doc.QuerySelector(".price-box-price");
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
