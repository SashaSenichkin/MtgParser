﻿using System.Text;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Text;
using Microsoft.Extensions.Configuration;
using MtgParser.Context;
using MtgParser.Model;

using IAngleConfig = AngleSharp.IConfiguration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;


namespace MtgParser.ParseLogic;

public class ParseService
{
    private readonly IConfigurationSection _urlsConfig;
    private readonly MtgContext _context;

    private const string CellSelectorMain = ".SearchCardInfoText";
    private const string CellSelectorInfo = ".SearchCardInfoDIV";
    private const string MtgRuInConfig = "BaseMtgRu";

    public ParseService(IConfiguration fullConfig)
    {
        _urlsConfig = fullConfig.GetSection("ExternalUrls");
        //_context = context;
    }
    
    public async Task<Card> ParseOneCard(string cardName)
    {
        try
        {
            IDocument doc = await GetCardInfo(cardName);
            Card result = ParseDoc(doc);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
    private async Task<IDocument> GetCardInfo(string cardName)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        IAngleConfig config = Configuration.Default.WithDefaultLoader();
        IBrowsingContext context = BrowsingContext.New(config);
        return await context.OpenAsync(_urlsConfig[MtgRuInConfig] + cardName);
    }
    
    private Card ParseDoc(IDocument doc)
    {
        IHtmlCollection<IElement> cellsText = doc.QuerySelectorAll(CellSelectorMain);
        
        IHtmlCollection<IElement> cellsInfo = doc.QuerySelectorAll(CellSelectorInfo);

        return SplitDataToFields(cellsText: cellsText, cellsInfo: cellsInfo);
    }
    
    private Card SplitDataToFields( IHtmlCollection<IElement> cellsText,  IHtmlCollection<IElement> cellsInfo)
    {
        IHtmlImageElement? img = cellsInfo[0].QuerySelectorAll("img").FirstOrDefault() as IHtmlImageElement;
        (string cmc, string color) cmcColor = GetManaCostAndColor(cellsInfo[2]);
        (string power, string toughness) powerAndTough = GetPowerAndToughness(cellsInfo[3]);
        (List<String> keywords, string text) keywordsAndText = GetKeywordsAndText(cellsInfo[0]);
        Card result = new()
        {
            Name = cellsInfo[0].TextContent.Trim(),
            Img = img?.Source,
            Type = GetSubstringAfterChar(cellsInfo[1].TextContent.Replace("\n", String.Empty),':'),
            Text = cellsText[0].TextContent,
            TextRus = cellsText[1].TextContent,
            Power = powerAndTough.power,
            Toughness = powerAndTough.toughness,
            Cmc =  cmcColor.cmc,
            Color = cmcColor.color
        };

        return result;
    }

    private (List<string> keywords, string text) GetKeywordsAndText(IElement element)
    {
        throw new NotImplementedException();
    }

    private string GetSubstringAfterChar(string text, params char[] separators)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (separators.Contains(text[i]))
                return text.Substring(i+1);
        }

        return text;
    }

    private (string power, string toughness) GetPowerAndToughness(IElement source)
    {
        string powerAndTough = GetSubstringAfterChar(source.TextContent, ':');
        int separator = powerAndTough.IndexOf('/');
        return (powerAndTough[..^separator].Trim(), powerAndTough[(separator + 1)..].Trim());
    }

    private (string cmc, string color) GetManaCostAndColor(IElement source)
    {
        IEnumerable<string> allData = source.QuerySelectorAll(".Mana")
            .Select(x => (x as IHtmlImageElement)?.AlternativeText)
            .Where(x => !String.IsNullOrEmpty(x))!;

        List<string> allColorData = allData.Where(x => x.All(y => !y.IsDigit())).ToList();

        String color = String.Join(", ", allColorData);

        int cmc = allColorData.Count;

        if (Int32.TryParse(allData.FirstOrDefault(x => x.All(y => y.IsDigit())), out int digit))
            cmc += digit;

        return (cmc.ToString(), color);
    }
}