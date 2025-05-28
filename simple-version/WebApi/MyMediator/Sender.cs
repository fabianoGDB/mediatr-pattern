using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.MyMediator
{
    public class Sender(IServiceProvider serviceProvider) : ISender
    {
        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            dynamic handler = serviceProvider.GetRequiredService(handlerType);

            return handler.Handle((dynamic)request, cancellationToken);
        }
    }
}