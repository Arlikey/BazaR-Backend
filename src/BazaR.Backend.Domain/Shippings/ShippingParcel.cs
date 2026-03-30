using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Shippings;

public sealed record ShippingParcel
{
    public string Description { get; private set; } = default!;
    public decimal InsuranceCost { get; private set; }
    public int RowNumber { get; private set; }
    public decimal Width { get; private set; }
    public decimal Length { get; private set; }
    public decimal Height { get; private set; }
    public decimal ActualWeight { get; private set; }
    public decimal VolumetricWeight { get; private set; }

    // 🔥 НОВОЕ ПОЛЕ
    public string CargoCategory { get; private set; } = "parcel";

    private ShippingParcel() { }

    private ShippingParcel(
        string description,
        decimal insuranceCost,
        int rowNumber,
        decimal width,
        decimal length,
        decimal height,
        decimal actualWeight,
        decimal volumetricWeight)
    {
        Description = description;
        InsuranceCost = insuranceCost;
        RowNumber = rowNumber;
        Width = width;
        Length = length;
        Height = height;
        ActualWeight = actualWeight;
        VolumetricWeight = volumetricWeight;

        // 🔥 гарантируем
        CargoCategory = "parcel";
    }

    public static Result<ShippingParcel> Create(
        string description,
        decimal insuranceCost,
        int rowNumber,
        decimal width,
        decimal length,
        decimal height,
        decimal actualWeight,
        decimal? volumetricWeight = null,
        string cargoCategory = null)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return Result<ShippingParcel>.Failure(new Error(
                "ShippingParcel.Description.Required",
                "Parcel description is required."));
        }

        if (insuranceCost < 0)
        {
            return Result<ShippingParcel>.Failure(new Error(
                "ShippingParcel.InsuranceCost.Invalid",
                "Insurance cost cannot be negative."));
        }

        if (rowNumber <= 0)
        {
            return Result<ShippingParcel>.Failure(new Error(
                "ShippingParcel.RowNumber.Invalid",
                "Row number must be greater than zero."));
        }

        if (width <= 0)
        {
            return Result<ShippingParcel>.Failure(new Error(
                "ShippingParcel.Width.Invalid",
                "Width must be greater than zero."));
        }

        if (length <= 0)
        {
            return Result<ShippingParcel>.Failure(new Error(
                "ShippingParcel.Length.Invalid",
                "Length must be greater than zero."));
        }

        if (height <= 0)
        {
            return Result<ShippingParcel>.Failure(new Error(
                "ShippingParcel.Height.Invalid",
                "Height must be greater than zero."));
        }

        if (actualWeight <= 0)
        {
            return Result<ShippingParcel>.Failure(new Error(
                "ShippingParcel.ActualWeight.Invalid",
                "Actual weight must be greater than zero."));
        }

        var calculatedVolumetricWeight =
            volumetricWeight ?? CalculateVolumetricWeight(width, length, height);

        if (calculatedVolumetricWeight <= 0)
        {
            return Result<ShippingParcel>.Failure(new Error(
                "ShippingParcel.VolumetricWeight.Invalid",
                "Volumetric weight must be greater than zero."));
        }

        return Result<ShippingParcel>.Success(new ShippingParcel(
            description.Trim(),
            insuranceCost,
            rowNumber,
            width,
            length,
            height,
            actualWeight,
            calculatedVolumetricWeight));
    }

    public Result Update(
        string description,
        decimal insuranceCost,
        int rowNumber,
        decimal width,
        decimal length,
        decimal height,
        decimal actualWeight,
        decimal? volumetricWeight = null)
    {
        var createResult = Create(
            description,
            insuranceCost,
            rowNumber,
            width,
            length,
            height,
            actualWeight,
            volumetricWeight);

        if (createResult.IsFailure)
            return Result.Failure(createResult.Error);

        var parcel = createResult.Value!;

        Description = parcel.Description;
        InsuranceCost = parcel.InsuranceCost;
        RowNumber = parcel.RowNumber;
        Width = parcel.Width;
        Length = parcel.Length;
        Height = parcel.Height;
        ActualWeight = parcel.ActualWeight;
        VolumetricWeight = parcel.VolumetricWeight;

        //гарантируем всегда
        CargoCategory = "parcel";

        return Result.Success();
    }

    private static decimal CalculateVolumetricWeight(
        decimal width,
        decimal length,
        decimal height)
    {
        return Math.Round((width * length * height) / 4000m, 3, MidpointRounding.AwayFromZero);
    }
}