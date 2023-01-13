using MtgParser.Model;

namespace MtgParser.Provider;

public interface ICardSetProvider
{
    Task<CardSet> GetCardSetAsync(CardName cardName);
    
    Task<CardSet> GetDataFromWebAsync(CardName cardName, Card? storedCard, Set? storedSet);
}