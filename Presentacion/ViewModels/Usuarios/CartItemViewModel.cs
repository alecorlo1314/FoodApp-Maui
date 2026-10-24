using CommunityToolkit.Mvvm.ComponentModel;
using FoodApp.Domain.Entities;

namespace FoodApp.Presentacion.ViewModels.Usuarios;

public partial class CartItemViewModel : ObservableObject
{
    public CartItemViewModel(CartItem item)
    {
        Id = item.Id;
        Name = item.Name;
        ImageUrl = item.ImageUrl;
        SizeName = item.SizeName;
        Price = item.Price;
        quantity = item.Quantity;
    }

    public long Id { get; }
    public string Name { get; }
    public string ImageUrl { get; }
    public string SizeName { get; }
    public decimal Price { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalItemPrice))]
    [NotifyPropertyChangedFor(nameof(TotalItemPriceFormatted))]
    private int quantity;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotUpdating))]
    private bool isUpdating;

    public bool IsNotUpdating => !IsUpdating;
    public decimal TotalItemPrice => Price * Quantity;
    public string TotalItemPriceFormatted => $"${TotalItemPrice:F0}";
}
