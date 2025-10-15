using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.ValueObjects
{
    public record Address
    {
     public string FirstName { get; init; } = string.Empty;
     public string LastName { get; init; } = string.Empty;
    public string? EmailAddress { get; init; } = string.Empty;
    public string AddressLine { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;

    }
}
