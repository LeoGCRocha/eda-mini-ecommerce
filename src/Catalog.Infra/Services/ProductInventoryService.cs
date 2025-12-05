using Catalog.Contracts.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Entities.InventoryItems;
using Catalog.Domain.Entities.Products;
using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infra.Services;

public class ProductInventoryService(
    IProductRepository productRepository,
    IInventoryItemRepository inventoryItemRepository,
    CatalogContext context)
    : IProductInventoryService
{
    // <WARNING> Talvez uma repository não tenha sido o esquema que tenha feito mais sentido aqui
    // uma lógica como uma service poderia ter encaixado melhor
    public async Task CreateProductAndInventoryAsync(Product product, int availableQuantity, int reorderQuantity)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            await productRepository.AddProductAsync(product);
            var inventory = new InventoryItem(product.Id, availableQuantity, reorderQuantity);
            await inventoryItemRepository.AddInventoryItemAsync(inventory);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeactivateProductAsync(ProductId productId)
    {
        var product = await productRepository.GetProductAsync(productId);
        if (product is null)
            // Should be a non-generic exception.
            throw new GenericException("Product not found.");
        product.DeactivateProduct();
        
        await context.SaveChangesAsync();
    }

    public async Task DeactivateProductOnInventoryAsync(ProductId productId)
    {
        var inventoryByProduct = await inventoryItemRepository.GetInventoryItemByProductId(productId);
        if (inventoryByProduct is null)
            throw new Exception("Não foi encontrado um inventario associado ao produto.");
        inventoryByProduct.MakeUnavailable();
        await context.SaveChangesAsync();
    }

    public async Task<List<ProductAvailabilityResponse>> HasAvailabilityForProduct(Dictionary<ProductId, int> productsWithQuantity)
    {
        // <COMMENT> Using this to avoid performing two round-trips to the database.
        var productIdsString = string.Join(",", productsWithQuantity.Keys.Select(id => $"'{id.Value}'"));
        
        var sql = $@"
        SELECT P.id as product_id, 
               P.base_price as unit_price, 
               IT.available_quantity
        FROM catalog.products P
        INNER JOIN catalog.inventory_items IT ON IT.product_id = P.id
        WHERE P.id IN ({productIdsString})
        ";

        var result = await context.Database.SqlQueryRaw<ProductWithAvailableAndPrice>(sql).ToListAsync();

        return result.Select(r => new ProductAvailabilityResponse(
            r.ProductId,
            r.AvailableQuantity >= productsWithQuantity[new ProductId(r.ProductId)],
            r.AvailableQuantity,
            r.UnitPrice
        )).ToList();
    }

    public async Task<bool> ReserveProductIfAvailable(ProductId productId, int quantity, OrderId orderId)
    {
        var inventoryItem = await inventoryItemRepository.GetInventoryItemByProductId(productId);
        if (inventoryItem is null)
            throw new GenericException("Não foi possível encontrar o item associado ao produto.");
        
        try
        {
            var reserveQuantity = inventoryItem.ReserveQuantity(quantity, orderId);
            await context.SaveChangesAsync();
            return reserveQuantity;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task CancelProductReservation(OrderId orderId, ProductId productId, int quantity)
    {
        var inventoryItem = await inventoryItemRepository.GetInventoryItemByProductId(productId);
     
        if (inventoryItem is null)
            throw new GenericException("Não foi possível encontrar o item associado ao produto.");

        inventoryItem.CancelReservation(orderId, quantity);

        await context.SaveChangesAsync();
    }

    public async Task ConfirmProductReservation(OrderId orderId, ProductId productId, int quantity)
    {
        var inventoryItem = await inventoryItemRepository.GetInventoryItemByProductId(productId);

        if (inventoryItem is null)
            throw new GenericException("Wasn't found any inventory item associated with this product.");

        inventoryItem.ConfirmReservation(orderId, quantity);

        await context.SaveChangesAsync();
    }

    private class ProductWithAvailableAndPrice
    {
        public Guid ProductId { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}