namespace BazaR.Backend.Api.Contracts.Reviews;

public sealed class CreateProductReviewRequest
{
    public Guid ProductId { get; init; }
    public int Rating { get; init; }
    public string Title { get; init; } = default!;
    public string Body { get; init; } = default!;
}