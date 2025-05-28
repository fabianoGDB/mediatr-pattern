using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.MyMediator;

namespace WebApi.Features
{

    public static class GetAllBooks
    {
        public record Query : IRequest<List<BookDTO>>;

        public class Handler : IRequestHandler<Query, List<BookDTO>>
        {
            public Task<List<BookDTO>> Handle(Query request, CancellationToken cancellationToken = default)
            {
                var books = new List<BookDTO>
            {
                new BookDTO { Id = 1, Title = "Book 1"},
                new BookDTO { Id = 2, Title = "Book 2"}
            };

                return Task.FromResult(books);
            }
        }

    }
}