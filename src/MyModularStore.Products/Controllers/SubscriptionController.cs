using Microsoft.AspNetCore.Mvc;
using MyModularStore.Products.Infrastructure;

namespace MyModularStore.Products.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController(
        SubscriptionPolicyRepository policyRepository,
        ILogger<SubscriptionController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> ValidateAsync(
            [FromQuery] string? key,
            [FromQuery] string? method,
            [FromQuery] string? path,
            CancellationToken ct = default)
        {
            logger.LogInformation("Validate called — key={Key} method={Method} path={Path}",
                key, method, path);

            if (string.IsNullOrWhiteSpace(key) ||
                string.IsNullOrWhiteSpace(method) ||
                string.IsNullOrWhiteSpace(path))
            {
                logger.LogWarning("Validate rejected — missing required query params");
                return BadRequest(new { error = "key, method, and path are required." });
            }

            var policy = await policyRepository.GetByKeyAsync(key, ct);

            if (policy is null)
            {
                return StatusCode(403, new { error = "Subscription key not found." });
            }

            if (!policy.IsActive)
            {
                return StatusCode(403, new { error = "Subscription is inactive." });
            }

            var requestSignature = $"{method.ToUpperInvariant()} {path}";
            var allowed = policy.AllowedEndpoints.Any(endpoint =>
                requestSignature.StartsWith(endpoint, StringComparison.OrdinalIgnoreCase));

            if (!allowed)
            {
                return StatusCode(403, new { error = "Your subscription does not have access to this endpoint.", tier = policy.Tier });
            }

            return Ok(new { allowed = true, tier = policy.Tier });
        }
    }
}
