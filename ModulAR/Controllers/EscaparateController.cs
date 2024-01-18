using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModulAR.Data;

namespace ModulAR.Controllers
{
    public class EscaparateController : Controller
    {
        private readonly MvcTiendaContexto _context;

        public EscaparateController(MvcTiendaContexto context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId, string strCadenaBusqueda)
        {
            ViewData["BusquedaActual"] = strCadenaBusqueda;

            // Cargar datos de las categorías
            var categorias = await _context.Categorias.ToListAsync();
            ViewData["Categorias"] = categorias;

            // Cargar datos de los productos
            var productosQuery = _context.Productos.AsQueryable();

            if (categoryId.HasValue)
            {
                productosQuery = productosQuery.Where(p => p.CategoriaId == categoryId);
            }

            var productos = await productosQuery.ToListAsync();

            ViewData["Productos"] = productos; // Agregar esta línea

            return View(productos);
        }
    }
}
