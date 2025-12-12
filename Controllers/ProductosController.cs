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

        // ============================
        //      INDEX + FILTROS
        // ============================
        public async Task<IActionResult> Index(string buscar, string estado)
        {
            var productos = _context.Productos.AsQueryable();

            // FILTRO POR ESTADO
            if (estado == "activos")
            {
                productos = productos.Where(p => p.Activo);
            }
            else if (estado == "inactivos")
            {
                productos = productos.Where(p => !p.Activo);
            }

            // FILTRO DE BÚSQUEDA
            if (!string.IsNullOrEmpty(buscar))
            {
                productos = productos.Where(p => p.Nombre.ToLower().Contains(buscar.ToLower()));
            }

            // Mantener valores en ViewBag
            ViewBag.Estado = estado;
            ViewBag.Buscar = buscar;

            return View(await productos.ToListAsync());
        }

        // ============================
        //          CREATE
        // ============================
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
                producto.FechaRegistro = DateTime.Now;  // Se mantiene la fecha automatizada

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(producto);
        }


        // ============================
        //           EDIT
        // ============================
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

            // Obtener registro existente
            var productoOriginal = await _context.Productos.FirstOrDefaultAsync(p => p.ProductoId == id);

            if (productoOriginal == null)
                return NotFound();

            // NO tocar la fecha
            productoOriginal.FechaRegistro = productoOriginal.FechaRegistro;

            // Actualizar propiedades editables
            productoOriginal.Nombre = producto.Nombre;
            productoOriginal.Descripcion = producto.Descripcion;   // <<< YA INCLUIDO
            productoOriginal.Precio = producto.Precio;
            productoOriginal.Stock = producto.Stock;
            productoOriginal.Activo = producto.Activo;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // ============================
        //           DELETE
        // ============================
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
                return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // ============================
        //       ESTADISTICAS
        // ============================
        public IActionResult Estadisticas()
        {
            var productos = _context.Productos.Where(p => p.Activo);

            ViewBag.PromedioPrecio = productos.Any() ? productos.Average(p => p.Precio) : 0;
            ViewBag.ValorInventario = productos.Any() ? productos.Sum(p => p.Precio * p.Stock) : 0;
            ViewBag.StockCritico = productos.Where(p => p.Stock < 5).ToList();
            ViewBag.ProductosCaros = productos.OrderByDescending(p => p.Precio).ToList();

            return View();
        }
    }
}
