using MtgParser.Model;

namespace MtgParser.Provider;

public interface IPriceProvider
{
    Task<Price> GetPriceAsync(CardSet cardSet);
}