using Microsoft.EntityFrameworkCore;
using InventarioMVC.Models;

namespace InventarioMVC.Data
{
    public class InventarioContext : DbContext
    {
        public InventarioContext(DbContextOptions<InventarioContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
    }
}
