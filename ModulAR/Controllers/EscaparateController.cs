using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModulAR.Data;
using ModulAR.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            var productosQuery = _context.Productos.Where(p => p.Escaparate == true) // Solo productos con Escaparate = true
            .AsQueryable();

            if (categoryId.HasValue)
            {
                productosQuery = productosQuery.Where(p => p.CategoriaId == categoryId);
            }
            var productos = await productosQuery.ToListAsync();

            ViewData["Productos"] = productos;

            return View(productos);
        }

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
        public async Task<IActionResult> AgregarCarrito(int id, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            // Recuperar el número de pedido de la sesión
            string numPedido = HttpContext.Session.GetString("NumPedido");

            if (string.IsNullOrEmpty(numPedido))
            {
                // Crear un nuevo pedido si no hay un número de pedido en la sesión
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

                // Asignar el número de pedido a la variable de sesión
                HttpContext.Session.SetString("NumPedido", nuevoPedido.Id.ToString());
                numPedido = nuevoPedido.Id.ToString();
            }

            // Crear un nuevo detalle y agregarlo a la base de datos
            var nuevoDetalle = new Detalle
            {
                PedidoId = Convert.ToInt32(numPedido),
                ProductoId = id,
                Cantidad = cantidad,
                Precio = producto.Precio
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
