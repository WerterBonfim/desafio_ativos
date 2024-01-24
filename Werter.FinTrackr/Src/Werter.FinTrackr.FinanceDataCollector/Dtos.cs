namespace Werter.FinTrackr.FinanceDataCollector;


public sealed record RequestResultApi
{
    public Chart Chart { get; set; }
    public object Erro { get; set; }
}

public sealed record Chart
{
    public List<ResultYahoo> Result { get; set; }
}

public sealed record ResultYahoo
{
    public List<long> Timestamp { get; set; }
    public Indicators Indicators { get; set; }
}

public sealed record Indicators
{
    public List<Quote> Quote { get; set; }
}

public sealed record Quote
{
    public List<double?> Open { get; set; }
}