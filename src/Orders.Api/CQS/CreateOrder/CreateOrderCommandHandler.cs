using System.Diagnostics;
using Catalog.Application.Observability;
using MediatR;
using Catalog.Contracts.DTOs;
using Orders.Domain.Entities;
using Orders.Domain.Services;
using Catalog.Contracts.Public;
using Microsoft.Extensions.Logging;
using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using Source = Orders.Application.Observability.Source;

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
        using Activity? activity = Source.OrderSource.StartActivity($"{nameof(CreateOrderCommandHandler)} : Creating new order", ActivityKind.Server);
        
        try
        {
            if (request.OrderItemDtos.FirstOrDefault(prod => prod.DesireQuantity == 0) is not null)
                throw new GenericException("All products must have a minimum quantity grather than 1.");
            
            var productAvailability = await _catalogApi.HasProductsAvailable(request.OrderItemDtos
                .Select(c => new ProductAvailabilityRequest(c.ProductId, c.DesireQuantity)).ToList());

            var unavailableProducts = productAvailability.Where(p => !p.AvailableForQuantity).ToList();

            if (unavailableProducts.Count != 0) 
                throw new GenericException("At least one product doesn't meet the minimum quantity requirement.");

            var orderItems = request.OrderItemDtos
                .Select(item => new OrderItem(new ProductId(item.ProductId), item.DesireQuantity,
                    productAvailability.First(inner => inner.ProductId == item.ProductId).UnitPrice)).ToList();

            var order = new Order(new CustomerId(request.CustomerId), orderItems);

            activity?.SetTag("order.id", order.Id);
            activity?.SetTag("order.amount", order.TotalAmount);
            activity?.SetTag("order.customer_id", order.CustomerId);
            
            await _orderService.CreateOrderAsync(order);

            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (GenericException ex)
        {
            _logger.LogWarning("Falha na validação do pedido: {Message}", ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Algo errado aconteceu ao tentar criar a order.");
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }
    }
}