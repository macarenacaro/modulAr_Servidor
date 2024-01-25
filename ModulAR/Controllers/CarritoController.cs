//carrito 4.0
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModulAR.Data;
using ModulAR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModulAR.Controllers
{
    [Authorize]
    public class CarritoController : Controller
    {
        private readonly MvcTiendaContexto _context;

        public CarritoController(MvcTiendaContexto context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var numPedido = HttpContext.Session.GetString("NumPedido");

          if (string.IsNullOrEmpty(numPedido))
            {
                return RedirectToAction(nameof(CarritoVacio));
            }

            var detalles = await _context.Detalles
                .Include(d => d.Producto)
                .Where(d => d.PedidoId == int.Parse(numPedido))
                .ToListAsync();

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == int.Parse(numPedido));

            //mostrar los detalles y pedido
            ViewData["Detalles"] = detalles;
            ViewData["Pedido"] = pedido;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarPedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null || pedido.EstadoId != 1)
            {
                // El pedido no existe o no está en estado "En carrito"
                return NotFound();
            }

            // Confirmar el pedido
            pedido.EstadoId = 2; // Cambiar el estado a "Confirmado"
            pedido.Confirmado = DateTime.Now;

            await _context.SaveChangesAsync();

            // Eliminar la variable de sesión NumPedido
            HttpContext.Session.Remove("NumPedido");

            // Redirigir al escaparate
            return RedirectToAction("Index", "Escaparate");
        }

        public IActionResult CarritoVacio()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EliminarLinea(int id)
        {
            var detalle = await _context.Detalles.FindAsync(id);

            if (detalle != null)
            {
                _context.Detalles.Remove(detalle);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MasCantidad(int id)
        {
            var detalle = await _context.Detalles.FindAsync(id);

            if (detalle != null)
            {
                detalle.Cantidad++;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MenosCantidad(int id)
        {
            var detalle = await _context.Detalles.FindAsync(id);

            if (detalle != null && detalle.Cantidad > 1)
            {
                detalle.Cantidad--;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

