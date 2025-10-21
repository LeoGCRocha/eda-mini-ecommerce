using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Application.Saga.States;

public class StateData
{
    public int ExpectedReservations { get; set; }
    public int CurrentReservations { get; set; }
    public int FailedReservations { get; set; }
    public List<ProductInformation> AlreadyReserved { get; set; } = new();
}

public class ProductInformation
{
    public ProductId ProductId { get; set; }
    public int Quantity { get; set; }
}