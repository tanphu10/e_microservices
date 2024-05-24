using AutoMapper;
using MediatR;
using Ordering.Application.Common.Exceptions;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Ordering.Domain.Entities;
using Serilog;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, ApiResult<OrderDto>>
    {
        private const string MethodName = "UpdateOrderCommandHandler";
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        public UpdateOrderCommandHandler(IOrderRepository orderRepository,
      IMapper mapper,
      ILogger logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<ApiResult<OrderDto>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = await _orderRepository.GetByIdAsync(request.Id);
            if (orderEntity is null) throw new NotFoundException(nameof(Order), request.Id);
            _logger.Information($"BEGIN: {MethodName} - Username: {request.LastName}");
            orderEntity = _mapper.Map(request, orderEntity);
            var updateOrder = await _orderRepository.UpdateOrderAsync(orderEntity);
            await _orderRepository.SaveChangesAsync();

            _logger.Information(
                $"Order {updateOrder.Id} was successfully updated.");
            var result = _mapper.Map<OrderDto>(updateOrder);

            _logger.Information($"END: {MethodName} - Username: {request.LastName}");
            return new ApiSuccessResult<OrderDto>(result);
        }
    }
}
