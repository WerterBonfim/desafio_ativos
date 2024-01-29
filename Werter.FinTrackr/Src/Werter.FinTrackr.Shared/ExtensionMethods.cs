namespace Werter.FinTrackr.Shared;

public static class ExtensionMethods
{
    public static decimal RoundMonetaryValue(this decimal value) 
        => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}