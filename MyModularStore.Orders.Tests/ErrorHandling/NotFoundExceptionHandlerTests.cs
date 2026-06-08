using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MyModularStore.Shared.ErrorHandling.Handlers;
using MyModularStore.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Orders.Tests.ErrorHandling
{
    public class NotFoundExceptionHandlerTests
    {
        private readonly NotFoundExceptionHandler _sut = new();


        [Fact]
        public async Task HandleAsync_SetsStatusCodeTo404()
        {
            //Arrange
            var context = BuildHttpContext();

            // Act
            await _sut.HandleAsync(context, new NotFoundException("not found"));

            //Assert
            context.Response.StatusCode.Should().Be(404);
        }


        private static DefaultHttpContext BuildHttpContext()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            return context;
        }
    }
}
