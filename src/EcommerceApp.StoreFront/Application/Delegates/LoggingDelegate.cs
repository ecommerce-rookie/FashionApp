using StoreFront.Domain.Models.Common;

namespace StoreFront.Application.Delegates;

public sealed class LoggingDelegate(ILogger<LoggingDelegate> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        try
        {            
            logger.LogInformation("[{Delegate}] Request: {RequestMethod} {RequestUri}", nameof(LoggingDelegate),
                request.Method, request.RequestUri);
            logger.LogInformation("[{Delegate}] Response: {ResponseStatusCode} {ResponseContent}",
                nameof(LoggingDelegate), response.StatusCode,
                
            await response.Content.ReadAsStringAsync(cancellationToken));

        } catch (HttpRequestException ex)
        {
            logger.LogError(ex, "[{Delegate}] {RequestMethod} - {RequestUri} has error: {ErrorMessage}",
                nameof(LoggingDelegate), request.Method,
                request.RequestUri, ex.Message);

            await response.Content.ReadAsStringAsync(cancellationToken);
        }

        return response;
    }
}