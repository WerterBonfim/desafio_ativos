using FluentResults;

namespace Werter.FinTrackr.Shared;

public static class Constants
{
    public const string DbName = "FinTrackrDb";
    public static readonly Result InternalError = Result.Fail("Ocorreu um erro interno no sistema. Tente novamente mais tarde ou contate o administrador");
}