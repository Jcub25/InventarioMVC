using Microsoft.AspNetCore.Mvc;
using InventarioMVC.Data;
using InventarioMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioMVC.Controllers
{
    public class ProductosController : Controller
    {
        private readonly InventarioContext _context;

        public ProductosController(InventarioContext context)
        {
            _context = context;
        }

        // ✅ LISTADO + BUSQUEDA
        public async Task<IActionResult> Index(string buscar)
        {
            var productos = from p in _context.Productos
                            where p.Activo
                            select p;

            if (!string.IsNullOrEmpty(buscar))
            {
                productos = productos.Where(p => p.Nombre.Contains(buscar));
            }

            return View(await productos.ToListAsync());
        }

        // ✅ CREATE
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                producto.FechaRegistro = DateTime.Now; // <<< Agregar esta línea

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(producto);
        }


        // ✅ EDIT
        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (id != producto.ProductoId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(producto);

            // Obtener registro original (tracking ON)
            var productoOriginal = await _context.Productos
                                                 .FirstOrDefaultAsync(p => p.ProductoId == id);

            if (productoOriginal == null)
                return NotFound();

            // Mantener la fecha original (no la toca)
            productoOriginal.FechaRegistro = productoOriginal.FechaRegistro;

            // Actualizar SOLO los campos editables
            productoOriginal.Nombre = producto.Nombre;
            productoOriginal.Descripcion = producto.Descripcion;
            productoOriginal.Precio = producto.Precio;
            productoOriginal.Stock = producto.Stock;
            productoOriginal.Activo = producto.Activo;

            // Guardar cambios
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // ✅ DELETE

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();   // ← Evita enviar null a Remove()
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // ✅ ESTADÍSTICAS
        public IActionResult Estadisticas()
        {
            var productos = _context.Productos.Where(p => p.Activo);

            ViewBag.PromedioPrecio = productos.Average(p => p.Precio);
            ViewBag.ValorInventario = productos.Sum(p => p.Precio * p.Stock);
            ViewBag.StockCritico = productos.Where(p => p.Stock < 5).ToList();
            ViewBag.ProductosCaros = productos.OrderByDescending(p => p.Precio).ToList();

            return View();
        }
    }
}
