using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoApi.Data;
using DemoApi.Models;

[Route("api/[controller]")]
[ApiController]
public class PedidosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PedidosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/pedidos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
    {
        return await _context.Pedidos.ToListAsync();
    }

    // POST: api/pedidos
    [HttpPost]
    public async Task<ActionResult<Pedido>> PostPedido(PedidoDto pedidodto)
    {
        Pedido pedido = new Pedido();
        pedido.Fecha = pedidodto.Fecha;
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetPedidos", new { id = pedido.Id }, pedido);
    }
}