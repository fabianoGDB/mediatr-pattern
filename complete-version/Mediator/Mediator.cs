using Mediator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mediator
{
    public class Mediator(IServiceProvider serviceProvider) : IMediator
    {
        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var requestType = request.GetType();
            var handlerType = typeof(IHandler<,>).MakeGenericType(requestType, typeof(TResponse));

            var handler = serviceProvider.GetService(handlerType);

            if (handler is null)
                throw new InvalidOperationException($"Handler not found for {requestType}");

            var method = handlerType.GetMethod("HandleAsync");

            if (method is null)
                throw new InvalidOperationException($"Method not found for {requestType}");

            var result = method.Invoke(handler, [request, cancellationToken]);

            if(result is not Task<TResponse> task)
                throw new InvalidOperationException($"Method returned unexpected type {result}");

            return await task;

        }
    }
}