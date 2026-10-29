using System;
using System.Collections.Generic;
using System.Text;

namespace FoodApp.Aplicacion.Features.Restaurants.Queries.GetRestaurants
{
    public sealed record RestaurantDto(
        long Id,
        string Name = "",
        string ImageUrl = "",
        List<string>? Tags = null,
        double Rating = 0,
        string DeliveryFee = "",
        string DeliveryTime = "",
        string Description = ""
    )
    {
        public string TagsFormatted => string.Join(" • ", Tags ?? []);
    }
}
