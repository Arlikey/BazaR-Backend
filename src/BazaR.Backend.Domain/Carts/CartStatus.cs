using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Domain.Carts
{
    public enum CartStatus
    {
        Active = 0,
        CheckedOut = 1,
        Abandoned = 2
    }
}
