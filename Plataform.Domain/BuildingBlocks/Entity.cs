namespace EdaMicroEcommerce.Domain.BuildingBlocks;

public abstract class Entity<T> where T : new()
{
    public T Id { get; set; }

    protected Entity(T id)
    {
        Id = id;
    }

    protected Entity()
    {
        Id = new T();
    }
}