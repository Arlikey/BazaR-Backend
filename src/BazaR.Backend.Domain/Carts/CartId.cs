using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace BazaR.Backend.Domain.Carts;

public readonly record struct CartId(Guid Value)
{
    public static CartId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}