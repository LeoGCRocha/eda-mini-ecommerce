namespace Billing.Domain.Entities;

public class Coupon
{
    public required string Name { get; init; }
    public bool IsActive { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public DateTime ValidUntilUtl { get; private set; }

    public Coupon(string name, bool isActive, decimal discountPercentage, DateTime validUntilUtl)
    {
        Name = name;
        IsActive = isActive;
        DiscountPercentage = discountPercentage;
        ValidUntilUtl = validUntilUtl;
    }
}