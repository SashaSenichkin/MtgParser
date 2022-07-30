using System.Text;
using AngleSharp;
using AngleSharp.Dom;

namespace MtgParser;

public class ParseLogic
{
    private const string baseUrl = "http://www.mtg.ru/cards/search.phtml?Title=";
    static async Task Main(string[] args)
    {

        if (args.Length == 0)
        {
            Console.WriteLine("название карты передать аргументом");
#if DEBUG
            var doc = await GetCardInfo("Пустыня");
            var result = ParceDoc(doc);
            File.WriteAllText("test.txt", result.ToString());
#endif
            return;
        }
        try
        {
            var doc = await GetCardInfo(args[0]);
            var result = ParceDoc(doc);
            File.WriteAllText("test.txt", result.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    private static CardInfo ParceDoc(IDocument doc)
    {
        string cellSelectorMain = ".SearchCardInfoText";
        IHtmlCollection<IElement> cellsText = doc.QuerySelectorAll(cellSelectorMain);
        
        string cellSelectorInfo = ".SearchCardInfoDIV";
        IHtmlCollection<IElement> cellsInfo = doc.QuerySelectorAll(cellSelectorInfo);

        return new CardInfo(cellsText: cellsText, cellsInfo: cellsInfo);
    }

    private static async Task<IDocument> GetCardInfo(string cardName)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        IConfiguration config = Configuration.Default.WithDefaultLoader();
        IBrowsingContext context = BrowsingContext.New(config);
        return await context.OpenAsync(baseUrl + cardName);
    }
}
