﻿using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Ordering.Domain.Entities;
using Serilog;
using Shared.SeedWork;


namespace Ordering.Application.Features.V1.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ApiResult<OrderDto>>
    {
        private const string MethodName = "CreateOrderCommandHandler";
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        public CreateOrderCommandHandler(IOrderRepository orderRepository,
       IMapper mapper,
       ILogger logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<ApiResult<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.Information($"BEGIN: {MethodName} - Username: {request.UserName}");
            var orderEntity = _mapper.Map<Order>(request);
            var addedOrder = await _orderRepository.CreateOrderAsync(orderEntity);
            orderEntity.AddedOrder();
            await _orderRepository.SaveChangesAsync();

            _logger.Information(
                $"Order {addedOrder.Id} - Document No: {addedOrder.DocumentNo} was successfully created.");
            var result = _mapper.Map<OrderDto>(orderEntity);

            _logger.Information($"END: {MethodName} - Username: {request.UserName}");

            return new ApiSuccessResult<OrderDto>(result);
        }
    }

}
