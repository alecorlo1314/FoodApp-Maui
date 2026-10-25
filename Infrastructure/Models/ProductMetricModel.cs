using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FoodApp.Infrastructure.Models;

[Table("product_metrics")]
public class ProductMetricModel : BaseModel
{
    // El SDK identifica la métrica por su clave única de negocio. PostgreSQL
    // genera id; omitirlo evita enviar el valor 0 al insertar varios productos.

    [PrimaryKey("user_id", true)]
    public Guid UserId { get; set; }

    [PrimaryKey("payment_id", true)]
    public string PaymentId { get; set; } = string.Empty;

    [PrimaryKey("product_id", true)]
    public long ProductId { get; set; }

    [Column("quantity_ordered")]
    public int QuantityOrdered { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
