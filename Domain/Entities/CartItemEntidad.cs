namespace FoodApp.Domain.Entities;

public sealed class CartItemEntidad
{
    public long Id { get; private set; }
    public Guid UserId { get; private set; }
    public long FoodId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string ImageUrl { get; private set; } = string.Empty;
    public string SizeName { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    public decimal TotalItemPrice => Price * Quantity;
    public string TotalItemPriceFormatted => $"${TotalItemPrice:F0}";

    // Constructor principal con validaciones de dominio estrictas
    public CartItemEntidad(
        Guid userId, 
        long foodId, 
        string name, 
        string imageUrl, 
        string sizeName, 
        decimal price, 
        int quantity, 
        long id = 0)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("El ID de usuario no puede estar vacío.", nameof(userId));

        if (foodId <= 0)
            throw new ArgumentException("El ID del alimento debe ser mayor a cero.", nameof(foodId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del producto no puede estar vacío.", nameof(name));

        if (price < 0)
            throw new ArgumentException("El precio no puede ser negativo.", nameof(price));

        if (quantity <= 0)
            throw new ArgumentException("La cantidad inicial debe ser mayor a cero.", nameof(quantity));

        Id = id;
        UserId = userId;
        FoodId = foodId;
        Name = name.Trim();
        ImageUrl = imageUrl?.Trim() ?? string.Empty;
        SizeName = sizeName?.Trim() ?? string.Empty;
        Price = price;
        Quantity = quantity;
    }

    public decimal CalculateTotalItemPrice() => TotalItemPrice;

    // Incrementar la cantidad de forma controlada
    public void IncreaseQuantity(int amount = 1)
    {
        if (amount <= 0)
            throw new ArgumentException("El incremento debe ser mayor a cero.", nameof(amount));

        UpdateQuantity(checked(Quantity + amount));
    }

    // Un ítem del carrito debe conservar al menos una unidad.
    public void DecreaseQuantity(int amount = 1)
    {
        if (amount <= 0)
            throw new ArgumentException("El decremento debe ser mayor a cero.", nameof(amount));

        if (Quantity - amount <= 0)
            throw new ArgumentException("La cantidad resultante debe ser mayor a cero.", nameof(amount));

        Quantity -= amount;
    }

    // Establecer una cantidad específica validando reglas de negocio
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(newQuantity));

        Quantity = newQuantity;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new ArgumentException("El precio no puede ser negativo.");
        Price = newPrice;
    }
}
