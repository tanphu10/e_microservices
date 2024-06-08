using MediatR;
using Ordering.Application.Common.Interfaces;
using Serilog;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders
{
    public class DeleteOrderByDocumentNoCommandHanlder : IRequestHandler<DeleteOrderByDocumentNoCommand, ApiResult<bool>>
    {

        private readonly IOrderRepository _orderRepository;
        private readonly ILogger _logger;
        public DeleteOrderByDocumentNoCommandHanlder(ILogger logger, IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }
        public async Task<ApiResult<bool>> Handle(DeleteOrderByDocumentNoCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = await _orderRepository.GetOrderByDocumentNo(request.DocumentNo);
            if (orderEntity == null) return new ApiResult<bool>(true);
            _orderRepository.DeleteOrder(orderEntity);
            orderEntity.DeleteOrder();
            await _orderRepository.SaveChangesAsync();
            _logger.Information($"Order {orderEntity.Id} was successfully deleted");
            return new ApiResult<bool>(true);
        }
    }
}
