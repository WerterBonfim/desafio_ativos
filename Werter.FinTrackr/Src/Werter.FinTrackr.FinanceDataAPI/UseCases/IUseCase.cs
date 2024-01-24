using FluentResults;

namespace Werter.FinTrackr.FinanceDataAPI.UseCases;

public interface IUseCase<in TInput, TOutput>
{
    Task<TOutput> ExecuteAsync(TInput input, CancellationToken cancellationToken);
}