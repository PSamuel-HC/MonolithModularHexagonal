using Grpc.Core;
using MyModularStore.Customers.Application.Ports;

namespace MyModularStore.Customers.GrpcServices
{
    public class CustomerGrpcService(ICustomerModule customerModule)
        : CustomerService.CustomerServiceBase
    {

        public override async Task<CustomerResponse> GetCustomer(
        GetCustomerRequest request, ServerCallContext context)
        {
            try
            {
                var customer = await customerModule.GetOneCustomerAsync(request.Id);
                return MapToResponse(customer);
            }
            catch
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Customer {request.Id} not found"));
            }
        }

        public override async Task ListCustomers(
        ListCustomersRequest request,
        IServerStreamWriter<CustomerResponse> responseStream,
        ServerCallContext context)
        {
            var customers = await customerModule.GetCustomersAsync();

            foreach (var customer in customers)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;

                await responseStream.WriteAsync(MapToResponse(customer));
            }
        }


        private static CustomerResponse MapToResponse(Application.DTOs.CustomerDto customer) =>
        new()
        {
            Id = customer.Id,
            FullName = customer.FullName,
            Email = customer.Email,
            IsPremium = customer.IsPremium,
            PointsBalance = customer.PointsBalance
        };

    }
}
