namespace EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

public class InventoryItemId : ValueObject
{
    public Guid Value { get; }

    public InventoryItemId(Guid value)
    {
        Value = value;
    }

    public InventoryItemId()
    {
        Value = Guid.NewGuid();
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}