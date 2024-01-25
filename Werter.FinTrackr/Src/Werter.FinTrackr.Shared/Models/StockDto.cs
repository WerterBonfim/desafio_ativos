namespace Werter.FinTrackr.Shared.Models;

public sealed record StockDto(string Name, double? Value, long Date)
{
    public DateTime DataAsDateTime => DateTimeOffset.FromUnixTimeSeconds(Date).DateTime;
}