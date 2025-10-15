using MediatR;
using Catalog.Contracts.DTOs;
using Orders.Domain.Entities;
using Orders.Domain.Services;
using Catalog.Contracts.Public;
using Microsoft.Extensions.Logging;
using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Api.CQS.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
{
    private readonly ICatalogApi _catalogApi;
    private readonly IOrderService _orderService;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(ICatalogApi catalogApi, IOrderService orderService,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _catalogApi = catalogApi;
        _orderService = orderService;
        _logger = logger;
    }

    public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pre validation to avoid overload on SAGA / Broker
            if (request.OrderItemDtos.FirstOrDefault(prod => prod.DesireQuantity == 0) is not null)
                throw new GenericException("Todos os produtos precisam ter quantidade maior que zero.");
            
            var productAvailability = await _catalogApi.HasProductsAvailable(request.OrderItemDtos
                .Select(c => new ProductAvailabilityRequest(c.ProductId, c.DesireQuantity)).ToList());

            var unavailableProducts = productAvailability.Where(p => !p.AvailableForQuantity).ToList();

            if (unavailableProducts.Count != 0) // TODO: Não deixar exceção generica.
                throw new GenericException("Pelo menos um produto não possui quantidade suficiente.");

            // TODO: Talvez um strategy ou uma mudança aqui pra quando for ter logica pra validação de CUPOM DE DESCONTO SE ENCAIXE.
            // TODO: Implementar CUPOM DE DESCONTO
            var orderItems = request.OrderItemDtos
                .Select(item => new OrderItem(new ProductId(item.ProductId), item.DesireQuantity,
                    productAvailability.First(inner => inner.ProductId == item.ProductId).UnitPrice)).ToList();

            var order = new Order(new CustomerId(request.CustomerId), orderItems);

            await _orderService.CreateOrderAsync(order);
        }
        catch (GenericException ex)
        {
            _logger.LogWarning("Falha na validação do pedido: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Algo errado aconteceu ao tentar criar a order.");
            throw;
        }
    }
}