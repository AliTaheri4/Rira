using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace CodeSample.gRPC.Interceptors;

public class ExceptionHandlingInterceptor : Interceptor
{
    private readonly ILogger<ExceptionHandlingInterceptor> _logger;
    public ExceptionHandlingInterceptor(ILogger<ExceptionHandlingInterceptor> logger) => _logger = logger;

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (RpcException) // قبلاً map شده
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation/argument error at {Method}", context.Method);
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "NotFound at {Method}", context.Method);
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict/invalid operation at {Method}", context.Method);
            throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message));
        }
        catch (Exception ex)
        {
            // لاگ کلی (خطای پیش‌بینی‌نشده)
            _logger.LogError(ex, "Unhandled exception at {Method}", context.Method);

            // می‌تونی متادیتا هم اضافه کنی (trace-id یا error-code)
            var trailers = new Metadata
            {
                { "error-code", "UNHANDLED" }
            };

            throw new RpcException(
                new Status(StatusCode.Internal, "Internal server error"),
                trailers);
        }
    }
}
