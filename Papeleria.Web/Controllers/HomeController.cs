using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Papeleria.Core.Data;
using Papeleria.Core.Services;

namespace Papeleria.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly PapeleriaDbContext _context;
        private readonly InventarioVentasService _service;

        public HomeController(PapeleriaDbContext context, InventarioVentasService service)
        {
            _context = context;
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos.ToListAsync();
            var ventas = await _context.Ventas.ToListAsync();

            ViewBag.TotalProductos = productos.Count;
            ViewBag.TotalUnidadesStock = productos.Sum(p => p.Stock);
            ViewBag.TotalVentasRealizadas = ventas.Count;
            ViewBag.IngresosTotales = ventas.Sum(v => v.TotalVenta);

            var colaReposicion = await _service.ObtenerColaReposicionAsync();
            return View(colaReposicion);
        }
    }
}
