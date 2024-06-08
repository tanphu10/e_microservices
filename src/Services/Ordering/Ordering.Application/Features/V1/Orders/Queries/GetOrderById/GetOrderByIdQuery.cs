using MediatR;
using Ordering.Application.Common.Models;
using Shared.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Features.V1.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery:IRequest<ApiResult<OrderDto>>
    {
        public long Id { get; private set; }
        public GetOrderByIdQuery(long id)
        {
            Id = id;
        }
    }
}
