using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModulAR.Data;
using ModulAR.Models;

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

        //SE MODIFICÓ PARA VENTANA EMERGENTE
        [HttpGet]
        public async Task<IActionResult> AgregarCarrito(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            return PartialView("AgregarCarrito", producto);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarCarritos(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            string numPedido = HttpContext.Session.GetString("NumPedido");

            if (string.IsNullOrEmpty(numPedido))
            {
                var nuevoPedido = new Pedido
                {
                    Fecha = DateTime.Now,
                    Confirmado = null,
                    Preparado = null,
                    Enviado = null,
                    Cobrado = null,
                    Devuelto = null,
                    Anulado = null,
                    ClienteId = 2, // Considera obtener el ClienteId de la sesión o de alguna otra manera
                    EstadoId = 1
                };

                if (ModelState.IsValid)
                {
                    _context.Add(nuevoPedido);
                    await _context.SaveChangesAsync();
                }

                HttpContext.Session.SetString("NumPedido", nuevoPedido.Id.ToString());
                numPedido = nuevoPedido.Id.ToString();
            }

            var nuevoDetalle = new Detalle
            {
                PedidoId = Convert.ToInt32(numPedido),
                ProductoId = id,
                Cantidad = 1,
                Precio = producto.Precio,
                Descuento = 0
            };

            if (ModelState.IsValid)
            {
                _context.Add(nuevoDetalle);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
