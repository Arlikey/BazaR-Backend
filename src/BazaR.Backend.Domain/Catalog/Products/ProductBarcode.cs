using System.Linq;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog.Products;

public sealed record ProductBarcode(string Value)
{
    public static Result<ProductBarcode> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<ProductBarcode>.Failure(new Error("Product.BarcodeRequired", "Barcode is required."));

        // оставим только цифры (часто штрих-код вводят с пробелами/дефисами)
        var digits = new string(value.Where(char.IsDigit).ToArray());

        // EAN-8 (8), UPC-A (12), EAN-13 (13), GTIN-14 (14)
        if (digits.Length is not (8 or 12 or 13 or 14))
            return Result<ProductBarcode>.Failure(new Error("Product.BarcodeInvalid", "Barcode must be EAN-8/UPC/EAN-13/GTIN-14."));

        return Result<ProductBarcode>.Success(new ProductBarcode(digits));
    }
}
