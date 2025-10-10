namespace EdaMicroEcommerce.Domain.BuildingBlocks;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public static bool operator ==(ValueObject? a, ValueObject? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(ValueObject? a, ValueObject? b) => !(a == b);
    
    public virtual bool Equals(ValueObject? other) =>
        other is not null && ValuesEquality(other);

    public override bool Equals(object? obj)
    {
        return obj is ValueObject valueObject && ValuesEquality(valueObject);
    }

    public override int GetHashCode()
    {
        return GetAtomicValues().Aggregate(
            0,
            (hashCode, value) => HashCode.Combine(hashCode, value.GetHashCode()));
    }

    protected abstract IEnumerable<object> GetAtomicValues();
    
    private bool ValuesEquality(ValueObject other) =>
        GetAtomicValues().SequenceEqual(other.GetAtomicValues());
}