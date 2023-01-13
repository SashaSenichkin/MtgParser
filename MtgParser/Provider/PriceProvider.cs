using AngleSharp.Dom;
using MtgParser.Context;
using MtgParser.Model;
using System.Text;
using AngleSharp;
using Microsoft.EntityFrameworkCore;
using MtgParser.ParseLogic;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;


namespace MtgParser.Provider;

/// <inheritdoc cref="MtgParser.Provider.IPriceProvider" />
public class PriceProvider : BaseProvider, IPriceProvider
{
    private readonly PriceParser _parser;
    private readonly IConfigurationSection _urlsConfig;

    /// <inheritdoc />
    public PriceProvider(PriceParser parser, IConfiguration fullConfig)
    {
        _urlsConfig = fullConfig.GetSection("ExternalUrls");
        _parser = parser;

    }

    /// <inheritdoc />
    public async Task<Price> GetPriceAsync(CardSet cardSet)
    {
        try
        {
            string searchCardName = cardSet.Card.Name.Replace(' ', '+');
            IDocument doc = await GetHtmlAsync($"{_urlsConfig["PriceApi"] + cardSet.Set.SearchText}/{searchCardName}" );
            Price result = PriceParser.GetPrice(cardSet, doc);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}