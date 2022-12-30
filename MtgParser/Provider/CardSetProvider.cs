using AngleSharp.Dom;
using MtgParser.Context;
using MtgParser.Model;
using System.Text;
using AngleSharp;
using Microsoft.EntityFrameworkCore;
using MtgParser.ParseLogic;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;


namespace MtgParser.Provider;

public class CardSetProvider : ICardSetProvider
{
    private readonly MtgContext _context;
    private readonly CardSetParser _parser;
    private readonly IConfigurationSection _urlsConfig;

    private const string MtgRuInConfig = "BaseMtgRu";
    private const string MtgRuInfoTableConfig = "MtgRuInfoTable";

    public CardSetProvider(MtgContext context, CardSetParser parser, IConfiguration fullConfig)
    {
        _urlsConfig = fullConfig.GetSection("ExternalUrls");
        _parser = parser;
        _context = context;
    }
    
    public async Task<CardSet> GetCardSetAsync(CardName cardName)
    {
        try
        {
            CardSet? storedCardSet = await GetCardSetFromDbAsync(cardName);
            if (storedCardSet != null )
            {
                return storedCardSet;
            }

            IDocument doc = await GetHtmlAsync(_urlsConfig[MtgRuInConfig] + cardName.SeekName + $"&Grp={cardName.SetShort}");
            Card card = _parser.GetCard(doc);
            if (card == null)
            {
                throw new Exception($"can't find card {cardName.SeekName}");
            }

            if (string.IsNullOrEmpty(cardName.SetShort))
            {
                return new CardSet() { Card = card };
            }
            
            Set set = await GetSetFromWebAsync(cardName.SetShort, doc);
            if (set.ShortName != cardName.SetShort)
            {
                throw new Exception($"Проверьте название сета предполагаемый вариант {set.ShortName}({set.FullName}) предложенный {cardName.SetShort}");
            }

            CardSet? result = await GetCardSetFromWebAsync(set, card, cardName, doc);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<Set?> GetSetFromWebAsync(string setShortName, IDocument doc)
    {
        (IEnumerable<string> variousSets, IElement defaultOption) candidates = CardSetParser.GetSetCandidates(doc);

        foreach (string cardVersion in candidates.variousSets)
        {
            IDocument docT = await GetHtmlAsync(_urlsConfig[MtgRuInfoTableConfig] + cardVersion);
            Set set = _parser.GetSet(docT);
            if (set.ShortName == setShortName)
            {
                return set;
            }
        }
        
        return _parser.GetSet(candidates.defaultOption);
    }

    private async Task<CardSet?> GetCardSetFromWebAsync(Set set, Card card, CardName cardName, IDocument doc)
    {
        CardSet result = _parser.GetCardSet(doc);
        result.Set = set;
        result.Card = card;
        result.Quantity = cardName.Quantity;
        result.IsFoil = (byte) (cardName.IsFoil ? 1 : 0);
        
        return result;
    }
    
    private async Task<Card?> GetCardFromDbAsync(string cardName)
    {
        Card? card = await _context.Cards.FirstOrDefaultAsync(x => 
            !string.IsNullOrEmpty(x.Name) && x.Name.Contains(cardName)
            || !string.IsNullOrEmpty(x.NameRus) && x.NameRus.Contains(cardName));
        return card;
    }
    
    private async Task<Set?> GetSetFromDbAsync(string setShortName)
    {
        Set? storedSet = await _context.Sets.FirstOrDefaultAsync(x => x.ShortName.Equals(setShortName));
        return storedSet;
    }
    
    private async Task<CardSet?> GetCardSetFromDbAsync(CardName cardName)
    {
        Set? set = await GetSetFromDbAsync(cardName.SetShort);
        Card? card = await GetCardFromDbAsync(cardName.SeekName);

        if (set == null || card == null)
        {
            return null;
        }
        
        CardSet? storedSet = await _context.CardsSets.FirstOrDefaultAsync(x => x.Card.Id == card.Id && x.Set.Id == set.Id);
        if (storedSet == null)
        {
            return null;
        }
        
        //TODO: add valid difference condition
        if ((storedSet.IsFoil == 1) == cardName.IsFoil && storedSet.Quantity == cardName.Quantity)
        {
            return storedSet;
        }

        CardSet result = new()
        {
            Set = set,
            Card = card,
            IsFoil = (byte)(cardName.IsFoil ? 1 : 0),
            Quantity = cardName.Quantity,
            Rarity = storedSet.Rarity,
        };
        
        return result;
    }
    
    private static async Task<IDocument> GetHtmlAsync(string path)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        AngleSharp.IConfiguration config = Configuration.Default.WithDefaultLoader();
        IBrowsingContext context = BrowsingContext.New(config);
        return await context.OpenAsync(path);
    }
}