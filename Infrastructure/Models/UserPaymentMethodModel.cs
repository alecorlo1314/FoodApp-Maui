using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using FoodApp.Aplicacion.Features.Payments.Queries.GetPaymentMethods;

namespace FoodApp.Infrastructure.Models;

[Table("user_payment_methods")]
public class UserPaymentMethodModel : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Column("card_holder_name")]
    public string CardHolderName { get; set; } = string.Empty;

    [Column("card_last_four")]
    public string CardLastFour { get; set; } = string.Empty;

    [Column("brand")]
    public string Brand { get; set; } = string.Empty;

    [Column("expiry_date")]
    public string ExpiryDate { get; set; } = string.Empty;

    [Column("is_default")]
    public bool IsDefault { get; set; }

    public UserPaymentMethodDto ToDto() => new()
    {
        Id = Id,
        UserId = UserId,
        CardHolderName = CardHolderName,
        CardNumberMasked = CardLastFour,
        Brand = Brand,
        ExpiryDate = ExpiryDate,
        IsDefault = IsDefault
    };
}