namespace BazaR.Backend.Api.Contracts.Reviews
{
   
    public sealed class EditProductReviewRequest
    {
        public int? Rating { get; init; }
        public string? Title { get; init; }
        public string? Body { get; init; }
    }
}
