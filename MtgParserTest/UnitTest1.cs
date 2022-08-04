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
    private ParseService _parseService;
    
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

        _dbContext.SaveChanges();
        
        var appSettings = @"{""ExternalUrls"": {""BaseMtgRu"": ""http://www.mtg.ru/cards/search.phtml?Title=""}}";

        var builder = new ConfigurationBuilder();
        builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

        var configuration= builder.Build();
        _parseService = new ParseService(configuration, _dbContext);
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
        Card card = _parseService.ParseDoc(doc);
        
        Assert.AreEqual("4", card.Toughness);
        Assert.AreEqual("5", card.Power);
        Assert.AreEqual("4", card.Cmc);
        Assert.AreEqual("R", card.Color);
    }
    
    [Test]
    public void CostXTest()
    {
        using IBrowsingContext context = BrowsingContext.New(Configuration.Default);

        IDocument doc = GetDocument("Gyrus Waker of Corpses.htm", context);
        Card card = _parseService.ParseDoc(doc);
        
        Assert.AreEqual("0", card.Toughness);
        Assert.AreEqual("0", card.Power);
        Assert.AreEqual("X, 3", card.Cmc);
        Assert.AreEqual("B, R, G", card.Color);
    }
    
    [Test]
    public void InfinityPowerTest()
    {
        using IBrowsingContext context = BrowsingContext.New(Configuration.Default);

        IDocument doc = GetDocument("Infinity Elemental.htm", context);
        Card card = _parseService.ParseDoc(doc);
        
        Assert.AreEqual("5", card.Toughness);
        Assert.AreEqual("/", card.Power);
        Assert.AreEqual("7", card.Cmc);
        Assert.AreEqual("R", card.Color);
    }
    
    [Test]
    public void NoPowerTest()
    {
        using IBrowsingContext context = BrowsingContext.New(Configuration.Default);

        IDocument doc = GetDocument("A Little Chat.htm", context);
        Card card = _parseService.ParseDoc(doc);
        
        Assert.AreEqual("-", card.Toughness);
        Assert.AreEqual("-", card.Power);
        Assert.AreEqual("2", card.Cmc);
        Assert.AreEqual("U", card.Color);
    }
    
    [Test]
    public void ArtifactTest()
    {
        using IBrowsingContext context = BrowsingContext.New(Configuration.Default);

        IDocument doc = GetDocument("Lucky Clove.htm", context);
        Card card = _parseService.ParseDoc(doc);
        
        Assert.AreEqual("-", card.Toughness);
        Assert.AreEqual("-", card.Power);
        Assert.AreEqual("2", card.Cmc);
        Assert.AreEqual("-", card.Color);
    }
    
    [Test]
    public void abstractLandTest()
    {
        using IBrowsingContext context = BrowsingContext.New(Configuration.Default);

        IDocument doc = GetDocument("Secluded Steppe.htm", context);
        Card card = _parseService.ParseDoc(doc);
        
        Assert.AreEqual("-", card.Toughness);
        Assert.AreEqual("-", card.Power);
        Assert.AreEqual("2", card.Cmc);
        Assert.AreEqual("-", card.Color);
    }
}