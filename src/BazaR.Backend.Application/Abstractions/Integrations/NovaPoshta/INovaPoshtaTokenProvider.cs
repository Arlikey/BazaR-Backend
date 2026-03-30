using System.Threading;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Abstractions.Integrations.NovaPoshta;

public interface INovaPoshtaTokenProvider
{
    Task<string> GetTokenAsync(CancellationToken ct = default);
}