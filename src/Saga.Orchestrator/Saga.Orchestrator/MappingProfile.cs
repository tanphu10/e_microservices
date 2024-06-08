using AutoMapper;
using Shared.Dtos.Basket;
using Shared.Dtos.Orders;

namespace Saga.Orchestrator
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<BasketCheckoutDto, CreateOrderDto>();
        }
    }
}
