using Contracts.Common.Interfaces;
using Infrastructure.Common.Repositories;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order, long, OrderContext>, IOrderRepository
    {
        public OrderRepository(OrderContext dbContext, IUnitOfWork<OrderContext> unitOfWork) : base(dbContext, unitOfWork)
        {

        }

        public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
     => await FindByCondition(x => x.UserName.Equals(userName)).ToListAsync();
        public async Task<Order> CreateOrderAsync(Order order)
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

        public async Task<Order> GetOrderById(long id)
        => await FindByCondition(x => x.Id.Equals(id)).FirstOrDefaultAsync();

        public async Task<Order> GetOrderByDocumentNo(string documentNo)
       => await FindByCondition(x => x.DocumentNo.ToString().Equals(documentNo)).FirstOrDefaultAsync();
    }

}
