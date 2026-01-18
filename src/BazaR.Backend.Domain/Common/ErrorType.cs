using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Domain.Common;

public enum ErrorType
{
    None = 0,
    Validation = 1,
    Conflict = 2,
    NotFound = 3,
    Forbidden = 4,
    Unauthorized = 5,
    Failure = 6
}
