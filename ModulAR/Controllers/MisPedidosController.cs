using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModulAR.Data;
using ModulAR.Models;

namespace ModulAR.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class MisPedidosController : Controller
    {
        private readonly MvcTiendaContexto _context;

        public MisPedidosController(MvcTiendaContexto context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int? pageNumber, decimal? minTotal, decimal? maxTotal)
        {
            var userEmail = User.Identity.Name;
            var cliente = await _context.Clientes.SingleOrDefaultAsync(c => c.Email == userEmail);

            if (cliente == null)
            {
                // Handle the case when the client does not exist
                return RedirectToAction("Create", "MisDatos");
            }

            IQueryable<Pedido> pedidosQuery = _context.Pedidos
                .Include(p => p.Detalles)
                .Include(p => p.Cliente)
                .Include(p => p.Estado)
                .Where(p => p.ClienteId == cliente.Id && p.EstadoId != 1);

            if (!string.IsNullOrEmpty(searchString))
            {
                pedidosQuery = pedidosQuery.Where(p => EF.Functions.Like(p.Id.ToString(), $"%{searchString}%") ||
                                                       EF.Functions.Like(p.Estado.Descripcion, $"%{searchString}%") ||
                                                       p.Detalles.Any(d => EF.Functions.Like(d.Producto.Descripcion, $"%{searchString}%")));
            }

            if (minTotal.HasValue)
            {
                pedidosQuery = pedidosQuery.Where(p => p.Detalles.Sum(d => d.Cantidad * d.Precio - d.Descuento) >= minTotal.Value);
            }

            if (maxTotal.HasValue)
            {
                pedidosQuery = pedidosQuery.Where(p => p.Detalles.Sum(d => d.Cantidad * d.Precio - d.Descuento) <= maxTotal.Value);
            }

            int pageSize = 7;
            int totalItems = await pedidosQuery.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ViewData["CurrentPage"] = pageNumber ?? 1;
            ViewData["TotalPages"] = totalPages;
            ViewData["MinTotal"] = minTotal;
            ViewData["MaxTotal"] = maxTotal;

            var pedidos = await pedidosQuery
                .Skip(((pageNumber ?? 1) - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(pedidos);
        }

        // GET: MisPedidos/Detalles/5
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null || _context.Pedidos == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Estado)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido == null)
            {
                return View();
            }

            var detalles = await _context.Detalles
                .Include(d => d.Producto)
                .Where(d => d.PedidoId == pedido.Id)
                .ToListAsync();

            ViewData["Detalles"] = detalles;

            return View(pedido);
        }
    }
}
