using AutoMapper;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Application.Ports;
using MyModularStore.Orders.Application.Services;
using MyModularStore.Orders.Application.Validators;
using MyModularStore.Orders.Domain.Entities;
using MyModularStore.Shared.Contracts;
using MyModularStore.Shared.Events;
using MyModularStore.Shared.Exceptions;

namespace MyModularStore.Orders.Tests.Services
{
    public class OrderServiceTests
    {
        //Mocks
        private readonly Mock<IOrderRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ICustomerContract> _mockCustomerContract;
        private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
        private readonly Mock<ILogger<OrderService>> _mockLogger;

        //Real
        private readonly OrderCreateDtoValidators _createValidator;
        private readonly OrderUpdateDtoValidators _updateValidator;

        //Target unit tests
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockRepository = new Mock<IOrderRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockCustomerContract = new Mock<ICustomerContract>();
            _mockPublishEndpoint = new Mock<IPublishEndpoint>();
            _mockLogger = new Mock<ILogger<OrderService>>();
            _createValidator = new OrderCreateDtoValidators();
            _updateValidator = new OrderUpdateDtoValidators();

            _orderService = new OrderService(
                _mockRepository.Object,
                _mockMapper.Object,
                _createValidator,
                _updateValidator,
                _mockCustomerContract.Object,
                _mockPublishEndpoint.Object,
                _mockLogger.Object);
        }


        [Fact]
        public async Task CreateAsync_CustomerDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            OrderCreateDto dto = new OrderCreateDto()
            {
                CustomerId = 99,
                OrderNumber = "ORD-TEST",
                TotalAmount = 100,
                Status = Domain.Enums.OrderStatus.Pending
            };

            _mockCustomerContract.Setup(c => c.ExistsAsync(99)).ReturnsAsync(false);

            // Act
            var act = () => _orderService.CreateAsync(dto);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("*99*");
        }

        [Fact]
        public async Task CreateAsync_ValidOrder_CallsRepositoryAddOnce()
        {
            // Arrange
            var dto = ValidOrderCreateDto();
            var order = new Order { Id = 1, CustomerId = 1, OrderNumber = "ORD-TEST" };

            _mockCustomerContract.Setup(c => c.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            _mockMapper.Setup(m => m.Map<Order>(dto)).Returns(order);
            _mockMapper.Setup(m => m.Map<OrderReadDto>(order)).Returns(new OrderReadDto { Id = 1 });

            // Act
            await _orderService.CreateAsync(dto);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockPublishEndpoint.Verify(p => p.Publish(It.IsAny<OrderPlacedEvent>()), Times.Once);
        }


        private static OrderCreateDto ValidOrderCreateDto() => new()
        {
            CustomerId = 1,
            OrderNumber = "ORD-TEST",
            TotalAmount = 150.00m,
            Status = Domain.Enums.OrderStatus.Pending
        };
    }
}
