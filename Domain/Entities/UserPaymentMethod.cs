namespace FoodApp.Domain.Entities;

public class UserPaymentMethod
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    public string CardNumberMasked { get; set; } = string.Empty; // Ej: "436" o "**** **** **** 436"
    public string Brand { get; set; } = string.Empty; // "Visa" o "Mastercard"
    public string ExpiryDate { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
