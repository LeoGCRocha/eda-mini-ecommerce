using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Application.Saga.States;

public class ProductInformation
{
    public ProductId ProductId { get; set; }
    public int Quantity { get; set; }
}