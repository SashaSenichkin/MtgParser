using MtgParser.Model;

namespace MtgParser.Provider;

/// <summary>
/// Price logic.. pretty raw, many white spots, but 60% Cards can handle
/// </summary>
public interface IPriceProvider
{
    /// <summary>
    /// Предельно просто. отправляем CardSet, получаем цену, если поиск по GoldFish что-то дал
    /// </summary>
    /// <param name="cardSet"></param>
    /// <returns></returns>
    Task<Price> GetPriceAsync(CardSet cardSet);
}