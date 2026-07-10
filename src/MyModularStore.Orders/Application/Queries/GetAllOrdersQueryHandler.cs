using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Application.Ports;
using MyModularStore.Orders.Domain.Entities;
using System.Text.Json;

namespace MyModularStore.Orders.Application.Queries
{
    public class GetAllOrdersQueryHandler(
        IOrderRepository repository,
        IDistributedCache cache,
        ILogger<GetAllOrdersQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderReadDto>>
    {
        private const string CacheKey = "orders:all";

        public async Task<IEnumerable<OrderReadDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            byte[]? bytes = await cache.GetAsync(CacheKey, cancellationToken);
            if (bytes is not null)
            {
                logger.LogInformation("Cache HIT — orders:all served from Redis");
                return JsonSerializer.Deserialize<IEnumerable<OrderReadDto>>(bytes)!;
            }

            logger.LogInformation("Cache MISS — querying database");

            IEnumerable<Order> orders = await repository.GetAllAsync(cancellationToken);
            IEnumerable<OrderReadDto> result = mapper.Map<IEnumerable<OrderReadDto>>(orders);

            var serialized = JsonSerializer.SerializeToUtf8Bytes(result);
            await cache.SetAsync(CacheKey, serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                }, cancellationToken);

            return result;
        }
    }
}
