namespace Orders.Application.Saga.States;

public class StateData
{
    public int ExpectedReservations { get; set; }
    public int CurrentReservations { get; set; }
    public int FailedReservations { get; set; }
    public List<ProductInformation> AlreadyReserved { get; set; } = new();
}