namespace EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

public class ProductId : ValueObject
{
    public Guid Value { get; set; }

    public ProductId(Guid value)
    {
        Value = value;
    }

    public ProductId()
    {
        Value = Guid.NewGuid();
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}