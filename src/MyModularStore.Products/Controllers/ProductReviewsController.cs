using Microsoft.AspNetCore.Mvc;
using MyModularStore.Products.Domain;
using MyModularStore.Products.Infrastructure;

namespace MyModularStore.Products.Controllers
{
    public record AddReviewDto(int CustomerId, int Rating, string Comment);

    [ApiController]
    [Route("api/products/{productId}/reviews")]
    public class ProductReviewsController(CosmosProductReviewRepository repository)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetReviews(string productId, CancellationToken ct)
        {
            var reviews = await repository.GetByProductIdAsync(productId, ct);
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(
            string productId,
            [FromBody] AddReviewDto dto,
            CancellationToken ct)
        {
            var review = new ProductReview
            {
                ProductId = productId,
                CustomerId = dto.CustomerId,
                Rating     = dto.Rating,
                Comment    = dto.Comment
            };
            await repository.AddAsync(review, ct);
            return CreatedAtAction(nameof(GetReviews), new { productId }, review);
        }
    }
}
