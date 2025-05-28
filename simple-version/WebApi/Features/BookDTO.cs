using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Features
{
    public record BookDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
    }
}