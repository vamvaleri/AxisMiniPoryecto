using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Papeleria.Core.Data;
using Papeleria.Core.Services;

namespace Papeleria.Web.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private readonly PapeleriaDbContext _context;
        private readonly InventarioVentasService _service;

        public VentasController(PapeleriaDbContext context, InventarioVentasService service)
        {
            _context = context;
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
            return View(ventas);
        }

        [HttpGet]
        public async Task<IActionResult> Nueva()
        {
            ViewBag.Productos = await _context.Productos.Where(p => p.Stock > 0).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Nueva(string nombreCliente, string metodoPago, int[] productoIds, int[] cantidades)
        {
            var items = new Dictionary<int, int>();
            for (int i = 0; i < productoIds.Length; i++)
            {
                if (cantidades[i] > 0)
                {
                    items[productoIds[i]] = cantidades[i];
                }
            }

            var resultado = await _service.RegistrarVentaAsync(nombreCliente, metodoPago, items);
            if (!resultado.Exito)
            {
                TempData["Error"] = resultado.Mensaje;
                ViewBag.Productos = await _context.Productos.Where(p => p.Stock > 0).ToListAsync();
                return View();
            }

            TempData["Mensaje"] = resultado.Mensaje;
            return RedirectToAction(nameof(Detalle), new { id = resultado.VentaCreada!.Id });
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null) return NotFound();
            return View(venta);
        }
    }
}
