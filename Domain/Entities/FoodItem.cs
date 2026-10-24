namespace FoodApp.Domain.Entities
{
    public class FoodItem
    {
        public long Id { get; set; }
        public long RestaurantId { get; set; }
        public long RestaurantCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PriceFormatted => $"${Price:F2}";
    }
}