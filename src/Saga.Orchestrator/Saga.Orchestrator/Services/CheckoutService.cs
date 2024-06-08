using AutoMapper;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.Services.interfaces;
using Shared.Dtos.Basket;
using Shared.Dtos.Inventory;
using Shared.Dtos.Orders;
using ILogger = Serilog.ILogger;

namespace Saga.Orchestrator.Services
{
    public class CheckoutSagaService : ICheckoutSagaService
    {
        private readonly IOrderHttpRepository _orderHttpRepository;
        private readonly IBasketHttpRepository _basketHttpRepository;
        private readonly IInventoryHttpRepository _inventoryHttpRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CheckoutSagaService(IOrderHttpRepository orderHttpRepository, IBasketHttpRepository basketHttpRepository, IInventoryHttpRepository inventoryHttpRepository, IMapper mapper, ILogger logger)
        {
            _basketHttpRepository = basketHttpRepository;
            _orderHttpRepository = orderHttpRepository;
            _inventoryHttpRepository = inventoryHttpRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> CheckoutOrder(string username, BasketCheckoutDto basketCheckout)
        {
            // Get Cart Basket Http Repository
            _logger.Information($"Start: Get Cart {username}");
            var cart = await _basketHttpRepository.GetBasket(username);
            if (cart == null) return false;
            _logger.Information($"EndL Get Cart {username} success");

            // create order from orderhttpRepository
            _logger.Information($"Start: Create Order ");

            var order = _mapper.Map<CreateOrderDto>(basketCheckout);
            order.TotalPrice = cart.TotalPrice;
            // Get Order by order Id

            var orderId = await _orderHttpRepository.CreateOrder(order);
            if (orderId < 0) return false;
            var addedOrder = await _orderHttpRepository.GetOrder(orderId);

            _logger.Information($"End: Create Order success,Order Id:{orderId} -Document No -{addedOrder.DocumentNo}");

            var inventoryDocumentNo = new List<string>();
            bool result;


            try
            {

                //Sales Items from InventoryHttpRepository


                foreach (var item in cart.Items)
                {
                    _logger.Information($"Start: sale Item No{item.ItemNo}-quantity:{item.Quantity}");
                    var saleOrder = new SalesProductDto(addedOrder.DocumentNo.ToString(), item.Quantity);
                    saleOrder.SetItemNo(item.ItemNo);
                    var documentNo = await _inventoryHttpRepository.CreateSalesOrder(saleOrder);
                    inventoryDocumentNo.Add(documentNo);
                    _logger.Information($"End: salte Item No :{item.ItemNo}-Quantity:{item.Quantity}");


                };
                result = true;

            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                RollbackCheckoutOrder(username, addedOrder.Id, inventoryDocumentNo);
                result = false;
            }

            return result;

            // rollback checkout order



        }
        private async Task RollbackCheckoutOrder(string username, long orderId, List<string> inventoryDocumentNos)
        {
            _logger.Information($"Start RollbackCheckoutOrder for username :{username}," + $"Order id:{orderId}" + $"inventory document no: {string.Join(",", inventoryDocumentNos)}");
            _logger.Information($"Start: Delete Order Id");
            await _orderHttpRepository.DeleteOrder(orderId);


            _logger.Information($"End: Deleted Order Id{orderId}");

            var deletedDocumentNos = new List<string>();
            // delete order by ỏrder's id , order's document no
            _logger.Information($"Start: Delete Inventory Document No");
            foreach (var documentNo in inventoryDocumentNos)
            {
                await _inventoryHttpRepository.DeleteOrderByDocumentNo(documentNo);
                deletedDocumentNos.Add(documentNo);
                _logger.Information($"End: Deleted Inventory Document Nos: {string.Join(",", inventoryDocumentNos)}");
            }
        }
    }
}
;