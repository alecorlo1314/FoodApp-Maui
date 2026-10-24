namespace FoodApp.Domain.Entities;

public class FoodSize
{
    public long Id { get; set; }
    public long FoodId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceModifier { get; set; }
}