using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Domain.Common;

/// <summary>
/// Domain error (business error, not technical).
/// Code should be stable because clients can rely on it.
/// </summary>
public sealed record Error(string Code, string Message, ErrorType Type = ErrorType.Failure)
{
    public static readonly Error None = new("", "", ErrorType.None);
    public bool IsNone => Type == ErrorType.None;
}