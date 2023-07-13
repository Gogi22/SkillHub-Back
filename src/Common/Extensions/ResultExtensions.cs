using Microsoft.AspNetCore.Http;

namespace Common.Extensions;

public static class ResultExtensions
{
    public static IResult ToActionResult(this Result result)
    {
        return !result.IsSuccess ? Results.BadRequest(result) : Results.Ok(result);
    }
}