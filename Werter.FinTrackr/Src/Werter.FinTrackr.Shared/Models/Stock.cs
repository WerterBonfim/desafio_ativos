namespace Werter.FinTrackr.Shared.Models;

public record Stock(int Id, string Name, decimal Value, DateTime Date);
public record StocksWithVariation(int Day, DateTime Date, decimal Value, decimal VariationD1, decimal VariationFirstDate);