using BazaR.Backend.Domain.Common;

public sealed record PhoneNumber
{
    public string Value { get; }
    public static int MaxLength { get; set; }

    private PhoneNumber(string value) => Value = value;

    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<PhoneNumber>.Failure(new Error("Seller.PhoneRequired", "Phone is required."));

        var v = value.Trim();
      
        if (v.Length < 7 || v.Length > 20)
            return Result<PhoneNumber>.Failure(new Error("Seller.PhoneInvalid", "Phone number length is invalid."));

        return Result<PhoneNumber>.Success(new PhoneNumber(v));
    }
}
