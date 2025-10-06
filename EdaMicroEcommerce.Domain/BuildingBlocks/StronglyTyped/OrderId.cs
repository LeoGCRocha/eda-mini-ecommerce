namespace EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

public class OrderId : ValueObject
{
    public Guid Value { get; set; }

    public OrderId(Guid value)
    {
        Value = value;
    }

    public OrderId()
    {
        Value = Guid.NewGuid();
    }
    
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}