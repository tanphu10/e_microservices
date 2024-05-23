﻿using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order, long, OrderContext>, IOrderRepository
    {
        public OrderRepository(OrderContext dbContext, IUnitOfWork<OrderContext> unitOfWork) : base(dbContext, unitOfWork)
        {

        }
        public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
     => await FindByCondition(x => x.UserName.Equals(userName)).ToListAsync();
        public async Task<Order> CreateOrder(Order order)
        {
            await CreateAsync(order);
            return order;
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            await UpdateAsync(order);
            return order;
        }

        public async void DeleteOrder(Order order)
        {
            await DeleteAsync(order);
        }
    }

}
