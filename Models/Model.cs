namespace DemoApi.Models
{
    public class PedidoDto
    {        
        public DateTime Fecha { get; set; } = DateTime.UtcNow; // Fecha
    }

    public class Pedido
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // UUID
        public DateTime Fecha { get; set; } = DateTime.UtcNow; // Fecha
    }
}