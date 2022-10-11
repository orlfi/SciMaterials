using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Exceptions;
using SciMaterials.Contracts.Result;

namespace SciMaterials.UI.MVC.API.Middlewares;

public class ErrorHandlerMiddleware
{
    private const string _defaultErrorMessage = "Application error";
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            await HandleErrorAsync(context, e);
        }
    }

    private async Task HandleErrorAsync(HttpContext Context, Exception exception)
    {
        _logger.LogError(exception, _defaultErrorMessage);

        Context.Response.Clear();

        var resultCode = exception switch
        {
            ApiException apiException => apiException.Code,
            _ => (int)ResultCodes.ServerError
        };

        var result = await Result<string>.ErrorAsync(resultCode, exception.Message);
        await Context.Response.WriteAsJsonAsync(result);
    }
}
