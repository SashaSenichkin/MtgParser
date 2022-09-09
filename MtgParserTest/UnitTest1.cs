using System.Text;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using MtgParser.Context;
using MtgParser.Model;
using MtgParser.ParseLogic;

namespace MtgParserTest;

public class Tests
{
    private MtgContext _dbContext;
    private ParseCardSet _parseCardSet;
    
    [SetUp]
    public void Setup()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
        var contextOptions = new DbContextOptionsBuilder<MtgContext>()
            .UseInMemoryDatabase("ReferencesProviderTest")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new MtgContext(contextOptions);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        _dbContext.Keywords.Add(new Keyword() {Name = "Indestructible", RusName = "Неразрушимость"});
        _dbContext.Keywords.Add(new Keyword() {Name = "haste", RusName = "Ускорение"});
        _dbContext.Keywords.Add(new Keyword() {Name = "Casualty", RusName = "Потери"});
        
        _dbContext.Rarities.Add(new Rarity() {Name = "Мythic", RusName = "Раритетная" });
        _dbContext.Rarities.Add(new Rarity() {Name = "Special", RusName = "Специальная" });
        _dbContext.Rarities.Add(new Rarity() {Name = "Uncommon", RusName = "Необычная" });
        _dbContext.Rarities.Add(new Rarity() {Name = "Common", RusName = "Обычная" });

        _dbContext.SaveChanges();
        
        var appSettings = @"{""ExternalUrls"": {""BaseMtgRu"": ""http://www.mtg.ru/cards/search.phtml"",""MtgRuInfoTable"": ""http://www.mtg.ru/cards/card_info_table.phtml?Action=ShowCardVersion&card=""}}";

        var builder = new ConfigurationBuilder();
        builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

        var configuration= builder.Build();
        _parseCardSet = new ParseCardSet(configuration, _dbContext);
    }

    private IDocument GetDocument(string sourceName, IBrowsingContext context)
    {
        const string upToProjectCases = "../../../TestCases";
        var fileName = Path.Combine(Environment.CurrentDirectory, upToProjectCases, sourceName);
        var sourceHtml = File.ReadAllText(fileName, Encoding.UTF8);
        
        var doc = context.OpenAsync(x => x.Content(sourceHtml));
        doc.Wait();
        
        return doc.Result;
    }

    [Test]
    public void HazoretTest()
    {
        using IBrowsingContext context = BrowsingContext.New(Configuration.Default);

        IDocument doc = GetDocument("Hazoret the Fervent.htm", context);
        Card card = _parseCardSet.GetParsedCard(doc);
        
        Assert.That(card.Toughness, Is.EqualTo("4"));
        Assert.That(card.Power, Is.EqualTo("5"));
        Assert.That(card.Cmc, Is.EqualTo("4"));
        Assert.That(card.Color, Is.EqualTo("R"));
    }
    
    [Test]
    public void CostXTest()
    {
        using IBrowsingContext context = BrowsingContext.New(Configuration.Default);

        IDocument doc = GetDocument("Gyrus Waker of Corpses.htm", context);
        Card card = _parseCardSet.GetParsedCard(doc);
        
        Assert.That(card.Toughness, Is.EqualTo("0"));
        Assert.That(card.Power, Is.EqualTo("0"));
        Assert.That(card.Cmc, Is.EqualTo("X, 3"));
        Assert.That(card.Color, Is.EqualTo("B, R, G"));
    }
    
    [Test]
    public void InfinityPowerTest()
    {
        using IBrowsingContext context = BrowsingContext.New(Configuration.Default);

        IDocument doc = GetDocument("Infinity Elemental.htm", context);
        Card card = _parseCardSet.GetParsedCard(doc);
        
        Assert.That(card.Toughness, Is.EqualTo("5"));
        Assert.That(card.Power, Is.EqualTo("/"));
        Assert.That(card.Cmc, Is.EqualTo("7"));
        Assert.That(card.Color, Is.EqualTo("R"));
    }
    
    [Test]
    public void NoPowerTest()
    {
        using IBrowsingContext context = BrowsingContext.New(Configuration.Default);

        IDocument doc = GetDocument("A Little Chat.htm", context);
        Card card = _parseCardSet.GetParsedCard(doc);
        
        Assert.That(card.Toughness, Is.EqualTo("-"));
        Assert.That(card.Power, Is.EqualTo("-"));
        Assert.That(card.Cmc, Is.EqualTo("2"));
        Assert.That(card.Color, Is.EqualTo("U"));
    }
    
    [Test]
    public void ArtifactTest()
    {
        using IBrowsingContext context = BrowsingContext.New(Configuration.Default);

        IDocument doc = GetDocument("Lucky Clove.htm", context);
        Card card = _parseCardSet.GetParsedCard(doc);
        
        Assert.That(card.Toughness, Is.EqualTo("-"));
        Assert.That(card.Power, Is.EqualTo("-"));
        Assert.That(card.Cmc, Is.EqualTo("2"));
        Assert.That(card.Color, Is.EqualTo("-"));
    }
    
    [Test]
    public void abstractLandTest()
    {
        using IBrowsingContext context = BrowsingContext.New(Configuration.Default);

        IDocument doc = GetDocument("Secluded Steppe.htm", context);
        Card card = _parseCardSet.GetParsedCard(doc);
        
        Assert.That(card.Toughness, Is.EqualTo("-"));
        Assert.That(card.Power, Is.EqualTo("-"));
        Assert.That(card.Cmc, Is.EqualTo("0"));
        Assert.That(card.Color, Is.EqualTo("-"));
    }
}