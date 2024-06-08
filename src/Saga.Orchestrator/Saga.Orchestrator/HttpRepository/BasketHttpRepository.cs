﻿using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.Dtos.Basket;

namespace Saga.Orchestrator.HttpRepository
{
    public class BasketHttpRepository : IBasketHttpRepository
    {
        private readonly HttpClient _client;
        public BasketHttpRepository(HttpClient client)
        {
            _client = client;
        }
        public async Task<bool> DeleteBasket(string username)
        {

            throw new NotImplementedException();

        }
        public async Task<CartDto> GetBasket(string username)
        {

            var cart = await _client.GetFromJsonAsync<CartDto>($"baskets/{username}");
            if (cart == null || !cart.Items.Any()) return null;
            return cart;

        }
    }
}
