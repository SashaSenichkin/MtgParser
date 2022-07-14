using System.Text;
using AngleSharp;
using AngleSharp.Dom;

namespace MtgParser;


class Program
{
    private const string baseUrl = "http://www.mtg.ru/cards/search.phtml?Title=";
    static async Task Main(string[] args)
    {
        try
        {
            var doc = await GetCardInfo(args);
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

    private static async Task<IDocument> GetCardInfo(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        IConfiguration config = Configuration.Default.WithDefaultLoader();
        IBrowsingContext context = BrowsingContext.New(config);
        return await context.OpenAsync(baseUrl + args[0]);
    }
}
