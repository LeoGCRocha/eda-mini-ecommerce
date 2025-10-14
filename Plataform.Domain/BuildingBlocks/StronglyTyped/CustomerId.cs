namespace EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

public class CustomerId : ValueObject
{
    public string Value { get; set; }

    public CustomerId(string value)
    {
        Value = value;
    }

    public CustomerId()
    {
        Value = string.Empty;
    }
    
    protected override IEnumerable<object> GetAtomicValues()
    {
        throw new NotImplementedException();
    }
}