namespace FoodApp.Domain.Entities;

public class Restaurant
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public double Rating { get; set; }
    public string DeliveryFee { get; set; } = string.Empty;
    public string DeliveryTime { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Propiedad calculada útil para mostrar los tags unidos por un punto en la UI
    public string TagsFormatted => string.Join(" • ", Tags);
}