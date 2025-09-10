using Microsoft.EntityFrameworkCore;
using DemoApi.Models;

namespace DemoApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        // aplication db context
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pedido> Pedidos { get; set; }
    }
}