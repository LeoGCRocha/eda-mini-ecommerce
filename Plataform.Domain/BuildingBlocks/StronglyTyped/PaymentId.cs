namespace EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

public class PaymentId : ValueObject
{
    public Guid Value { get; set; }

    public PaymentId(Guid value)
    {
        Value = value;
    }

    public PaymentId()
    {
        Value = Guid.NewGuid();
    }
    
    protected override IEnumerable<object> GetAtomicValues()
    {
        throw new NotImplementedException();
    }
}