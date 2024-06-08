﻿using Shared.Dtos.Inventory;

namespace Saga.Orchestrator.HttpRepository.Interfaces
{
    public interface IInventoryHttpRepository
    {
        Task<string> CreateSalesOrder(SalesProductDto model);
        Task<string> CreateOrderSales(string orderNo,SalesOrderDto model);

        Task<bool> DeleteOrderByDocumentNo(string documentNo);

    }
}
