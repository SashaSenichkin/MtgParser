using System.Text;
using ImageService.Context;
using ImageService.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace ImageServiceTest;

public class Tests
{
    private MtgContext _dbContext;
    private ImagesController _imagesController;
    private SelfCheckController _selfCheckController;
    
    [SetUp]
    public void Setup()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
        DbContextOptions<MtgContext> contextOptions = new DbContextOptionsBuilder<MtgContext>()
            .UseInMemoryDatabase("ImageServiceTest")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new MtgContext(contextOptions);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        _dbContext.Cards.Add(new Card(){Img = "http://www.mtg.ru/pictures/M20/FloodofTears1.jpg", IsRus = false, Id = 1});
        _dbContext.Cards.Add(new Card(){Img = "http://www.mtg.ru/pictures/M20_RUS/FloodofTears2.jpg", IsRus = false, Id = 2});
        _dbContext.Cards.Add(new Card(){Img = "http://www.mtg.ru/pictures/M20/FloodofTears3.jpg", IsRus = true, Id = 3});
        _dbContext.Cards.Add(new Card(){Img = "http://www.mtg.ru/pictures/M20_RUS/FloodofTears4.jpg", IsRus = true, Id = 4});

        _dbContext.Sets.Add(new Set() { SetImg = "http://www.mtg.ru/images2/sets/THB-U.gif" });
        _dbContext.Sets.Add(new Set() { SetImg = "http://www.mtg.ru/images2/sets/STA-U.gif" });
        
        _dbContext.SaveChanges();
        
        Mock<ILogger<ImagesController>> logger = new();
        
        _imagesController = new ImagesController(logger.Object, _dbContext);
        _selfCheckController = new SelfCheckController(logger.Object, _dbContext);
    }

    [Test]
    public void ImagesController()
    {
        _imagesController.SetImagesPaths("127.0.0.1:22/images").Wait();
        Assert.Multiple(() =>
        {
            Assert.That(_dbContext.Cards.First(x => x.Id == 1).Img, Is.EqualTo("127.0.0.1:22/images/M20/FloodofTears1.jpg"));
            Assert.That(_dbContext.Cards.First(x => x.Id == 1).IsRus, Is.EqualTo(false));
            Assert.That(_dbContext.Cards.First(x => x.Id == 2).Img, Is.EqualTo("127.0.0.1:22/images/M20_RUS/FloodofTears2.jpg"));
            Assert.That(_dbContext.Cards.First(x => x.Id == 3).Img, Is.EqualTo("127.0.0.1:22/images/M20/FloodofTears3.jpg"));
            Assert.That(_dbContext.Cards.First(x => x.Id == 3).IsRus, Is.EqualTo(true));
            Assert.That(_dbContext.Cards.First(x => x.Id == 4).Img, Is.EqualTo("127.0.0.1:22/images/M20_RUS/FloodofTears4.jpg"));
            
            Assert.That(_dbContext.Sets.First(x => x.Id == 1).SetImg, Is.EqualTo("127.0.0.1:22/images/sets/THB-U.gif"));
            Assert.That(_dbContext.Sets.First(x => x.Id == 2).SetImg, Is.EqualTo("127.0.0.1:22/images/sets/STA-U.gif"));
        });
    }
    
    [Test]
    public async Task SelfCheckController()
    {
        string[] errors = await _selfCheckController.CheckRusImages();
        Assert.Multiple(() =>
        {
            Assert.That(errors[0], Is.EqualTo("http://www.mtg.ru/pictures/M20/FloodofTears3.jpg"));
            Assert.That(errors[1], Is.EqualTo("http://www.mtg.ru/pictures/M20_RUS/FloodofTears2.jpg"));
        });
    }
}