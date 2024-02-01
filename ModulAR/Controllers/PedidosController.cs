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
using X.PagedList;

namespace ModulAR.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PedidosController : Controller
    {
        private readonly MvcTiendaContexto _context;

        public PedidosController(MvcTiendaContexto context)
        {
            _context = context;
        }

        // GET: Pedidos
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            ViewData["CurrentFilter"] = searchString;

            var pedidos = from p in _context.Pedidos
                          .Include(p => p.Cliente)
                          .Include(p => p.Estado)
                          select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                pedidos = pedidos.Where(p =>
                    p.Fecha.ToString().Contains(searchString) ||
                    p.Id.ToString().Contains(searchString) ||
                    p.Estado.Descripcion.Contains(searchString) ||
                    p.Cliente.Id.ToString().Contains(searchString)
                );
            }

            int pageSize = 7; // ajusta el tamaño de la página según tus necesidades
            int pageNumber = page ?? 1;

            return View(await pedidos.ToPagedListAsync(pageNumber, pageSize));
        }

        // GET: Pedidos/Details/5
        public async Task<IActionResult> Details(int? id)
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
                return NotFound();
            }

            return View(pedido);
        }

        // GET: Pedidos/Create
        public async Task<IActionResult> Create()
        {
            var emailUsuario = User.Identity.Name;
            var cliente = await _context.Clientes.SingleOrDefaultAsync(c => c.Email == emailUsuario);

            if (cliente == null)
            {
                // Si el cliente no existe, puedes manejarlo de acuerdo a tus necesidades
                // Por ejemplo, redireccionar a la acción de creación del cliente
                return RedirectToAction("Create", "MisDatos");
            }

            ViewData["ClienteId"] = cliente.Id; // Asignar el Id del cliente al ViewData
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Descripcion");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fecha,Confirmado,Preparado,Enviado,Cobrado,Devuelto,Anulado,ClienteId,EstadoId")] Pedido pedido)
        {
            // Asignar el Email del usuario actual
            var userEmail = User.Identity.Name;

            // Obtener el cliente correspondiente al usuario actual
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);

            if (cliente != null)
            {
                pedido.ClienteId = cliente.Id;
            }

            if (ModelState.IsValid)
            {
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", pedido.ClienteId);
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Descripcion", pedido.EstadoId);
            return View(pedido);
        }

        // GET: Pedidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pedidos == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", pedido.ClienteId);
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Descripcion", pedido.EstadoId);
            return View(pedido);
        }

        // POST: Pedidos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fecha,Confirmado,Preparado,Enviado,Cobrado,Devuelto,Anulado,ClienteId,EstadoId")] Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", pedido.ClienteId);
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Descripcion", pedido.EstadoId);
            return View(pedido);
        }

        // GET: Pedidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
                return NotFound();
            }

            // Verificar si el pedido está asociado a un cliente
            if (PedidoAsociadoACliente(pedido))
            {
                // Si está asociado, mostrar un mensaje de error
                TempData["ErrorMessage"] = "No se puede eliminar este pedido porque está asociado a un cliente.";
              // return RedirectToAction(nameof(Index));
            }

            return View(pedido);
        }

        // Método auxiliar para verificar si un pedido está asociado a un cliente
        private bool PedidoAsociadoACliente(Pedido pedido)
        {
            return _context.Clientes.Any(c => c.Pedidos.Any(p => p.Id == pedido.Id));
        }

        // POST: Pedidos/Delete/5
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pedidos == null)
            {
                return Problem("Entity set 'MvcTiendaContexto.Pedidos'  is null.");
            }

            var pedido = await _context.Pedidos.FindAsync(id);

            // Verificar nuevamente antes de eliminar
            if (PedidoAsociadoACliente(pedido))
            {
                // Si está asociado, mostrar un mensaje de error
                TempData["ErrorMessage"] = "No se puede eliminar este pedido porque está asociado a un cliente.";
                return RedirectToAction(nameof(Index));
            }

            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
          return (_context.Pedidos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
