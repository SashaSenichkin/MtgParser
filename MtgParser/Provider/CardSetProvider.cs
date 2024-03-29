﻿using System.Diagnostics.CodeAnalysis;
using AngleSharp.Dom;
using MtgParser.Context;
using MtgParser.Model;
using System.Text;
using AngleSharp;
using Microsoft.EntityFrameworkCore;
using MtgParser.ParseLogic;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;


namespace MtgParser.Provider;

/// <inheritdoc cref="MtgParser.Provider.ICardSetProvider" />
public class CardSetProvider : BaseProvider, ICardSetProvider
{
    private readonly MtgContext _context;
    private readonly CardSetParser _parser;
    private readonly IConfigurationSection _urlsConfig;

    private const string MtgRuInConfig = "BaseMtgRu";
    private const string MtgRuInfoTableConfig = "MtgRuInfoTable";

    /// <inheritdoc />
    public CardSetProvider(MtgContext context, CardSetParser parser, IConfiguration fullConfig)
    {
        _urlsConfig = fullConfig.GetSection("ExternalUrls");
        _parser = parser;
        _context = context;
    }


    /// <inheritdoc />
    public async Task<CardSet> GetCardSetAsync(CardName cardName)
    {
        try
        {
            Card? storedCard = await GetCardFromDbAsync(cardName.SeekName);
            Set? storedSet = await GetSetFromDbAsync(cardName.SetShort);
            
            if (storedCard != null && storedSet != null)
            {
                CardSet? storedCardSet = await GetCardSetFromDbAsync(cardName, storedSet, storedCard);
                if (storedCardSet != null )
                {
                    return storedCardSet;
                }
            }

            return await GetDataFromWebAsync(cardName, storedCard, storedSet);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<CardSet> GetDataFromWebAsync(CardName cardName, Card? storedCard = null, Set? storedSet = null)
    {
        IDocument doc = await GetHtmlAsync(_urlsConfig[MtgRuInConfig] + cardName.SeekName + $"&Grp={cardName.SetShort}");
        Card? card = storedCard ?? _parser.GetCard(doc, string.IsNullOrEmpty(cardName.Name));
        if (card == null)
        {
            throw new Exception(
                $"can't find card {cardName.SeekName} in set {cardName.SetShort} check address {doc.BaseUri}");
        }

        if (string.IsNullOrEmpty(cardName.SetShort))
        {
            return new CardSet { Card = card };
        }

        Set? set = storedSet ?? await GetSetFromWebAsync(cardName.SetShort, doc);
        if (set == null || set.ShortName != cardName.SetShort)
        {
            throw new Exception(
                $"Проверьте название сета найденный вариант {set.ShortName}({set.FullName}) запрошенный {cardName.SetShort}");
        }

        return GetCardSetFromWeb(set, card, cardName, doc);
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
    
    private CardSet GetCardSetFromWeb(Set set, Card card, CardName cardName, IDocument doc)
    {
        CardSet result = _parser.GetCardSet(doc);
        result.Set = set;
        result.Card = card;
        result.Quantity = cardName.Quantity;
        result.IsFoil = (byte) (cardName.IsFoil ? 1 : 0);
        
        return result;
    }
    
    private async Task<Card?> GetCardFromDbAsync(string? cardName)
    {
        if (string.IsNullOrEmpty(cardName))
        {
            return null;
        }
        
        Card? card = await _context.Cards.FirstOrDefaultAsync(x => 
            !string.IsNullOrEmpty(x.Name) && !x.IsRus && x.Name.Contains(cardName)
            || !string.IsNullOrEmpty(x.NameRus) && x.IsRus && x.NameRus.Contains(cardName));
        return card;
    }
    
    private async Task<Set?> GetSetFromDbAsync(string setShortName)
    {
        return await _context.Sets.FirstOrDefaultAsync(x => x.ShortName.Equals(setShortName));
    }

    private async Task<CardSet?> GetCardSetFromDbAsync(CardName cardName, Set set, Card card)
    {
        CardSet? storedSet = await _context.CardsSets
                                           .Include(x => x.Rarity)
                                           .FirstOrDefaultAsync(x => x.Card.Id == card.Id && x.Set.Id == set.Id);
        
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
}