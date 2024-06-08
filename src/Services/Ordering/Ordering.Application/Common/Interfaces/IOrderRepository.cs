using Ordering.Domain.Entities;
using Contracts.Domains.Interfaces;
using Contracts.Common.Interfaces;


namespace Ordering.Application.Common.Interfaces
{
    public interface IOrderRepository : IRepositoryBaseAsync<Order, long>
    {
        Task<IEnumerable<Order>> GetOrdersByUserName(string userName);
        Task<Order> GetOrderById(long id);

        Task<Order> GetOrderByDocumentNo(string documentNo);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> UpdateOrderAsync(Order order);
        void DeleteOrder(Order order);
    }
}
