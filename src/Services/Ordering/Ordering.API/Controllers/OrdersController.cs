using AutoMapper;
using Contracts.Messages;
using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders;
using Ordering.Application.Features.V1.Orders.Commands.CreateOrder;
using Ordering.Application.Features.V1.Orders.Commands.DeleteOrder;
using Ordering.Application.Features.V1.Orders.Commands.UpdateOrder;
using Ordering.Domain.Entities;
using Shared.Dtos.Orders;
using Shared.SeedWork;
using Shared.Services.Email;
using System.ComponentModel.DataAnnotations;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Ordering.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMessageProducer _messageProducer;
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;
        public OrdersController(IMessageProducer messageProducer, IOrderRepository repository, IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _messageProducer = messageProducer ?? throw new ArgumentNullException(nameof(messageProducer));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        private static class RouteNames
        {
            public const string GetOrders = nameof(GetOrders);
        }
        [HttpGet("{username}", Name = RouteNames.GetOrders)]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrderByUserName([Required] string username)
        {
            var query = new GetOrdersQuery(username);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDto request)
        {
            var order = _mapper.Map<Order>(request);
            var addedOrder = await _repository.CreateOrder(order);
            await _repository.SaveChangesAsync();
            var result = _mapper.Map<OrderDto>(addedOrder);
            _messageProducer.SendMessage(result);
            return Ok(result);
        }
    }









    #region CRUD

    //    public class OrdersController : ControllerBase
    //    {
    //        private readonly IMediator _mediator;
    //        private readonly IMapper _mapper;
    //        private readonly ISmtpEmailService _emailService;

    //        public OrdersController(IMediator mediator, IMapper mapper, ISmtpEmailService emailService)
    //        {
    //            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    //            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    //            _emailService = emailService;
    //        }

    //        private static class RouteNames
    //        {
    //            public const string GetOrders = nameof(GetOrders);
    //            public const string CreateOrder = nameof(CreateOrder);
    //            public const string UpdateOrder = nameof(UpdateOrder);
    //            public const string DeleteOrder = nameof(DeleteOrder);
    //        }
    //        [HttpGet("{username}", Name = RouteNames.GetOrders)]
    //        [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    //        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserName([Required] string username)
    //        {
    //            var query = new GetOrdersQuery(username);
    //            var result = await _mediator.Send(query);
    //            return Ok(result);
    //        }
    //        [HttpPost(Name = RouteNames.CreateOrder)]
    //        [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    //        public async Task<ActionResult<ApiResult<long>>> CreateOrder([FromBody] CreateOrderDto model)
    //        {
    //            var command = _mapper.Map<CreateOrderCommand>(model);

    //            var result = await _mediator.Send(command);
    //            return Ok(result);
    //        }
    //        [HttpPut("{id:long}", Name = RouteNames.UpdateOrder)]
    //        [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    //        public async Task<ActionResult<ApiResult<OrderDto>>> UpdateOrder([Required] long id, [FromBody] UpdateOrderCommand command)
    //        {
    //            command.SetId(id);
    //            var result = await _mediator.Send(command);
    //            return Ok(result);
    //        }
    //        [HttpDelete("{id:long}", Name = RouteNames.DeleteOrder)]
    //        [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.NoContent)]
    //        public async Task<ActionResult> DeleteOrder([Required] long id)
    //        {
    //            var command = new DeleteOrderCommand(id);
    //            await _mediator.Send(command);
    //            return NoContent();
    //        }

    //        [HttpGet("test-email")]
    //        public async Task<IActionResult> TestEmail()
    //        {
    //            var message = new MailRequest
    //            {
    //                Body = "<h1> hello </h1>",
    //                Subject = "English",
    //                ToAddress = "phantanphu899398@gmail.com"

    //            };
    //            await _emailService.SendEmailAsync(message);
    //            return Ok();
    //        }
    //    }

    //}


    #endregion
}