using Grpc.Core;
using Grpc.Core.Interceptors;
using LilCloudServerConsole.Services;

namespace LilCloudServerConsole.Classes
{
    public class JwtInterceptor : Interceptor
    {
        private readonly JwtTokenService _jwt;
        private readonly HashSet<string> _publicMethods = new HashSet<string>
        {
            "/cloud.LilCloud/AccessAccount",
            "/cloud.LilCloud/RegisterAccount"
        };
        public JwtInterceptor(JwtTokenService jwt)
        {
            _jwt = jwt;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            if (_publicMethods.Contains(context.Method))
            {
                return await continuation(request, context);
            }

            var token = context.RequestHeaders.FirstOrDefault(h => h.Key == "authorization")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "JWT Token missing"));
            }
            var (userId, isAdmin, username) = _jwt.ValidateToken(token);

            if (userId == null)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "No matching user found for this token"));
            }
            context.UserState["UserId"] = userId;
            context.UserState["IsAdmin"] = isAdmin;
            context.UserState["Username"] = username;

            return await continuation(request, context);
        }
        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            if (_publicMethods.Contains(context.Method))
            {
                await continuation(requestStream, responseStream, context);
                return;
            }

            var token = context.RequestHeaders.FirstOrDefault(h => h.Key == "authorization")?.Value;

            if (string.IsNullOrEmpty(token))
                throw new RpcException(new Status(StatusCode.Unauthenticated, "JWT Token missing"));

            var (userId, isAdmin, username) = _jwt.ValidateToken(token);

            if (userId == null)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "No matching user found for this token"));

            context.UserState["UserId"] = userId;
            context.UserState["IsAdmin"] = isAdmin;
            context.UserState["Username"] = username;

            await continuation(requestStream, responseStream, context);
        }
        //public bool PerformNullCheckVersion2(Person person)
        //{
        //    return person?.Head?.Nose?.Sniff() ?? false;
        //}
    }
}
