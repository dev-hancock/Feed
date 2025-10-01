using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Feed.Interceptors
{
    public class ExceptionInterceptor(ILogger<ExceptionInterceptor> logger) : Interceptor
    {
        private RpcException HandleException(Exception ex)
        {
            logger.LogError(ex, "Unhandled gRPC exception");

            var metadata = new Metadata
            {
                { "error-code", "INTERNAL_ERROR" },
                { "error-message", ex.Message }
            };

            return new RpcException(new Status(StatusCode.Internal, "An internal error occurred."), metadata);
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (Exception ex)
            {
                throw HandleException(ex);
            }
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
            TRequest request,
            IServerStreamWriter<TResponse> response,
            ServerCallContext context,
            ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                await continuation(request, response, context);
            }
            catch (Exception ex)
            {
                throw HandleException(ex);
            }
        }
    }
}
