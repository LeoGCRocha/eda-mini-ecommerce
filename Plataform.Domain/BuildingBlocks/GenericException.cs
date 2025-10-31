namespace EdaMicroEcommerce.Domain.BuildingBlocks;

public class GenericException : Exception
{
    public GenericException(string message) : base(message) { }
}