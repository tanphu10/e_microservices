using Grpc.Core;
using Inventory.Grpc.Protos;
using Inventory.Grpc.Repositories.Interfaces;
using ILogger = Serilog.ILogger;
namespace Inventory.Grpc.Services
{
    public class InventoryService : StockProtoService.StockProtoServiceBase
    {
        private readonly IInventoryRepository _repository;
        private readonly ILogger _logger;

        public InventoryService(ILogger logger, IInventoryRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public override async Task<StockModel> GetStock(GetStockRequest request, ServerCallContext context)
        {
            _logger.Information($"BEGIN Get Stock of itemNo :{request.ItemNo}");

            var stockQuantity = await _repository.GetStockQuantity(request.ItemNo);
            var result = new StockModel
            {
                Quantity = stockQuantity
            };
            _logger.Information($"END Get Stock of itemNo :{request.ItemNo}-Quantity:");
            return result;

        }
    }
}
