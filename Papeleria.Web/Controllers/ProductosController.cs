using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Papeleria.Core.Data;
using Papeleria.Core.Models;

namespace Papeleria.Web.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly PapeleriaDbContext _context;

        public ProductosController(PapeleriaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _context.Productos.OrderBy(p => p.Categoria).ToListAsync();
            return View(lista);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = $"Producto '{producto.Nombre}' agregado al inventario.";
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Reponer(int id, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null && cantidad > 0)
            {
                producto.ReponerStock(cantidad);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = $"Se agregaron {cantidad} unidades a '{producto.Nombre}'.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
