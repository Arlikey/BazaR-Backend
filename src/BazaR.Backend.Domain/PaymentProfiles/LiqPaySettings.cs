using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.PaymentProfiles;

public sealed class LiqPaySettings : ValueObject
{
    public string PublicKey { get; private set; } = default!;
    public string PrivateKey { get; private set; } = default!;
    public string? ResultUrl { get; private set; }
    public string? ServerCallbackUrl { get; private set; }

    public bool CheckoutEnabled { get; private set; }
    public bool PrivatPayEnabled { get; private set; }
    public bool InstallmentsEnabled { get; private set; }

    private LiqPaySettings() { }

    private LiqPaySettings(
        string publicKey,
        string privateKey,
        string? resultUrl,
        string? serverCallbackUrl,
        bool checkoutEnabled,
        bool privatPayEnabled,
        bool installmentsEnabled)
    {
        PublicKey = publicKey;
        PrivateKey = privateKey;
        ResultUrl = resultUrl;
        ServerCallbackUrl = serverCallbackUrl;
        CheckoutEnabled = checkoutEnabled;
        PrivatPayEnabled = privatPayEnabled;
        InstallmentsEnabled = installmentsEnabled;
    }

    public static Result<LiqPaySettings> Create(
        string publicKey,
        string privateKey,
        string? resultUrl,
        string? serverCallbackUrl,
        bool checkoutEnabled,
        bool privatPayEnabled,
        bool installmentsEnabled)
    {
        if (string.IsNullOrWhiteSpace(publicKey))
            return Result<LiqPaySettings>.Failure(PaymentProfileErrors.LiqPayPublicKeyRequired);

        if (string.IsNullOrWhiteSpace(privateKey))
            return Result<LiqPaySettings>.Failure(PaymentProfileErrors.LiqPayPrivateKeyRequired);

        return Result<LiqPaySettings>.Success(new LiqPaySettings(
            publicKey.Trim(),
            privateKey.Trim(),
            string.IsNullOrWhiteSpace(resultUrl) ? null : resultUrl.Trim(),
            string.IsNullOrWhiteSpace(serverCallbackUrl) ? null : serverCallbackUrl.Trim(),
            checkoutEnabled,
            privatPayEnabled,
            installmentsEnabled));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return PublicKey;
        yield return PrivateKey;
        yield return ResultUrl;
        yield return ServerCallbackUrl;
        yield return CheckoutEnabled;
        yield return PrivatPayEnabled;
        yield return InstallmentsEnabled;
    }
}