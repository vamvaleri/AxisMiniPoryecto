using Microsoft.EntityFrameworkCore;
using Papeleria.Core.Models;

namespace Papeleria.Core.Data
{
    public class PapeleriaDbContext : DbContext
    {
        public PapeleriaDbContext(DbContextOptions<PapeleriaDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Codigo = "CU-001", Nombre = "Cuaderno Argollado 100 Hojas", Categoria = "Cuadernos", PrecioVenta = 8500m, Stock = 25, StockMinimo = 5 },
                new Producto { Id = 2, Codigo = "ES-002", Nombre = "Bolígrafo Gel Negro 0.5mm", Categoria = "Escritura", PrecioVenta = 2500m, Stock = 50, StockMinimo = 10 },
                new Producto { Id = 3, Codigo = "PA-003", Nombre = "Resma Papel Bond Carta 75g", Categoria = "Papeles", PrecioVenta = 18000m, Stock = 12, StockMinimo = 4 },
                new Producto { Id = 4, Codigo = "AR-004", Nombre = "Caja Colores x24 Unidades", Categoria = "Arte", PrecioVenta = 22000m, Stock = 8, StockMinimo = 3 },
                new Producto { Id = 5, Codigo = "ES-005", Nombre = "Resaltador Pastel Unidad", Categoria = "Escritura", PrecioVenta = 3200m, Stock = 4, StockMinimo = 5 },
                new Producto { Id = 6, Codigo = "PA-006", Nombre = "Carpeta Legajadora Cartón", Categoria = "Archivo", PrecioVenta = 1800m, Stock = 30, StockMinimo = 8 }
            );

            modelBuilder.Entity<Cliente>().HasData(
                new Cliente { Id = 1, Nombre = "Cliente Mostrador", Documento = "000000", Telefono = "N/A" },
                new Cliente { Id = 2, Nombre = "Laura Gómez", Documento = "1144098765", Telefono = "3104567890" }
            );
        }
    }
}
