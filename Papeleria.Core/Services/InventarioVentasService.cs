using Microsoft.EntityFrameworkCore;
using Papeleria.Core.Data;
using Papeleria.Core.Models;

namespace Papeleria.Core.Services
{
    public class InventarioVentasService
    {
        private readonly PapeleriaDbContext _context;

        public InventarioVentasService(PapeleriaDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<int, Producto>> ObtenerMapaProductosAsync()
        {
            var productos = await _context.Productos.ToListAsync();
            var diccionario = new Dictionary<int, Producto>();
            foreach (var p in productos)
            {
                diccionario[p.Id] = p;
            }
            return diccionario;
        }

        public async Task<Queue<Producto>> ObtenerColaReposicionAsync()
        {
            var bajos = await _context.Productos
                .Where(p => p.Stock <= p.StockMinimo)
                .OrderBy(p => p.Stock)
                .ToListAsync();

            var cola = new Queue<Producto>();
            foreach (var item in bajos)
            {
                cola.Enqueue(item);
            }
            return cola;
        }

        public async Task<(bool Exito, string Mensaje, Venta? VentaCreada)> RegistrarVentaAsync(
            string nombreCliente,
            string metodoPago,
            Dictionary<int, int> itemsSolicitados)
        {
            if (itemsSolicitados == null || itemsSolicitados.Count == 0)
                return (false, "Debe seleccionar al menos un producto con cantidad mayor a 0.", null);

            var mapaProductos = await ObtenerMapaProductosAsync();
            var venta = new Venta
            {
                NombreClienteMostrador = string.IsNullOrWhiteSpace(nombreCliente) ? "Cliente Mostrador" : nombreCliente,
                MetodoPago = metodoPago,
                Fecha = DateTime.Now
            };

            foreach (var par in itemsSolicitados)
            {
                if (!mapaProductos.TryGetValue(par.Key, out var prod))
                    return (false, $"El producto ID {par.Key} no existe.", null);

                if (!prod.TieneStockDisponible(par.Value))
                    return (false, $"Stock insuficiente para '{prod.Nombre}'. Disponible: {prod.Stock} und.", null);
            }

            foreach (var par in itemsSolicitados)
            {
                var prod = mapaProductos[par.Key];
                prod.DescontarStock(par.Value);

                venta.Detalles.Add(new DetalleVenta
                {
                    ProductoId = prod.Id,
                    Cantidad = par.Value,
                    PrecioUnitario = prod.PrecioVenta,
                    Subtotal = prod.PrecioVenta * par.Value
                });
            }

            venta.CalcularTotal();
            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            return (true, $"Venta #{venta.Id} registrada por ${venta.TotalVenta:N0}.", venta);
        }
    }
}
