namespace EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

public class OrderItemId : ValueObject
{
    public Guid Value { get; set; }

    public OrderItemId(Guid value)
    {
        Value = value;
    }

    public OrderItemId()
    {
        Value = Guid.NewGuid();
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        throw new NotImplementedException();
    }
}