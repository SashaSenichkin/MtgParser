using MtgParser.Model;

namespace MtgParser.Provider;

/// <summary>
/// Main business logic for now.. took cardName, check with DB, load html, invoke parser 
/// </summary>
public interface ICardSetProvider
{
    /// <summary>
    /// checks Set from DB, then gets html and pass it to CardSetParser
    /// </summary>
    /// <param name="cardName">request object</param>
    /// <returns>Set in db already, but card and cardSet are your to proceed</returns>
    Task<CardSet> GetCardSetAsync(CardName cardName);
    
    /// <summary>
    /// Get All available data from web. Card, Set Rarity for CardSet
    /// </summary>
    /// <param name="cardName">search entity</param>
    /// <param name="storedCard">card from db if present</param>
    /// <param name="storedSet">set from db if present</param>
    /// <returns>CardSet parsed from web</returns>
    /// <exception cref="Exception">some shit happened.. no internet, of evil forces approach</exception>
    Task<CardSet> GetDataFromWebAsync(CardName cardName, Card? storedCard, Set? storedSet);
}