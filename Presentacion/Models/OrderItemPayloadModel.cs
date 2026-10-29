namespace FoodApp.Presentacion.Models;

// Modelo de apoyo para el transporte de datos
public class OrderItemPayloadModel
{
    public long ProductId { get; set; }
    public int Quantity { get; set; }
}
