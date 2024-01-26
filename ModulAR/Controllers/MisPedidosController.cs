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

        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity.Name;
            var cliente = await _context.Clientes.SingleOrDefaultAsync(c => c.Email == userEmail);

            if (cliente == null)
            {
                // Handle the case when the client does not exist
                return RedirectToAction("Create", "MisDatos");
            }

            var pedidos = await _context.Pedidos
                .Include(p => p.Detalles) // Asegúrate de incluir los detalles
                .Include(p => p.Cliente)
                .Include(p => p.Estado)
                .Where(p => p.ClienteId == cliente.Id)
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
