using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.ValueObjects
{
    public record Payment
    {
        public string CardName { get; init; } = string.Empty;
        public string CardNumber { get; init; } = string.Empty;
        public string Expiration { get; init; } = string.Empty;
        public string CVV { get; init; } = string.Empty;
        public int PaymentMethod { get; init; } = default!;

    }
}
