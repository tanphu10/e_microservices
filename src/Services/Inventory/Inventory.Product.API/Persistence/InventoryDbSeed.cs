﻿using Inventory.Product.API.Entities;
using Inventory.Product.API.Extensions;
using MongoDB.Driver;
using Shared.Configurations;

namespace Inventory.Product.API.Persistence
{
    public class InventoryDbSeed
    {
        public async Task SeedDataAsync(IMongoClient mongoClient, MongoDbSettings settings)
        {
            var databaseName = settings.DatabaseName;
            var database = mongoClient.GetDatabase(databaseName);
            var inventoryCollection = database.GetCollection<InventoryEntry>("InventoryEntries");
            if(await inventoryCollection.EstimatedDocumentCountAsync() == 0)
            {
                await inventoryCollection.InsertManyAsync(getPreConfiguredInventoryEntries());
            }
        }
        private IEnumerable<InventoryEntry> getPreConfiguredInventoryEntries()
        {
            return new List<InventoryEntry>
            {
                new()
                {
                    Quantity=10,
                    DocumentNo=Guid.NewGuid().ToString(),
                    ItemNo="Lotus",
                    ExternalDocumentNo=Guid.NewGuid().ToString(),
                    DocumentType=Shared.Enums.EDocumentType.Purchase,

                },
                 new()
                {
                    Quantity=10,
                    DocumentNo=Guid.NewGuid().ToString(),
                    ItemNo="Cadillac",
                    ExternalDocumentNo=Guid.NewGuid().ToString(),
                    DocumentType=Shared.Enums.EDocumentType.Purchase,

                },
            };
        }
    }
}