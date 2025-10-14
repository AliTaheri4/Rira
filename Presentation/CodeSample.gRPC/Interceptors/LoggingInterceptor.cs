using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace CodeSample.gRPC.Interceptors;

public class LoggingInterceptor : Interceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;
    public LoggingInterceptor(ILogger<LoggingInterceptor> logger) => _logger = logger;

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var sw = Stopwatch.StartNew();
        var method = context.Method;              // e.g. /persons.PersonService/CreatePerson
        var peer = context.Peer;                // e.g. ipv4:127.0.0.1:54321

        _logger.LogInformation("gRPC call started: {Method} from {Peer}", method, peer);

        try
        {
            var response = await continuation(request, context);
            sw.Stop();

            _logger.LogInformation(
                "gRPC call succeeded: {Method} in {Elapsed} ms",
                method, sw.ElapsedMilliseconds);

            return response;
        }
        catch (RpcException ex) // already mapped exception
        {
            sw.Stop();
            _logger.LogWarning(ex, "gRPC call failed (RPC): {Method} status={Status} in {Elapsed} ms",
                method, ex.StatusCode, sw.ElapsedMilliseconds);
            throw;
        }
        catch (Exception ex) // not mapped; ExceptionHandlingInterceptor هم بعد از این می‌گیرد
        {
            sw.Stop();
            _logger.LogError(ex, "gRPC call crashed: {Method} in {Elapsed} ms",
                method, sw.ElapsedMilliseconds);
            throw;
        }
    }
}