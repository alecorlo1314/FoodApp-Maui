namespace FoodApp.Domain.Entities;

public class OrderItemPayload
{
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime? CreatedAt { get; set; }
}
