namespace DemoApi.Models
{
    public class Pedido
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // UUID
        public DateTime Fecha { get; set; } = DateTime.UtcNow; // Fecha
    }
}