namespace BazaR.Backend.Api.Contracts.Reviews;

public sealed class CreateProductReviewRequest
{
    public Guid ProductId { get; init; }

    public int Rating { get; init; }

    public string? Advantages { get; init; }

    public string? Disadvantages { get; init; }

    public string? Body { get; init; }
}