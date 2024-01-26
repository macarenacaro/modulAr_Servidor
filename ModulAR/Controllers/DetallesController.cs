using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModulAR.Data;
using ModulAR.Models;

namespace ModulAR.Controllers
{
    public class DetallesController : Controller
    {
        private readonly MvcTiendaContexto _context;

        public DetallesController(MvcTiendaContexto context)
        {
            _context = context;
        }

        // GET: Detalles
        public async Task<IActionResult> Index()
        {
            var mvcTiendaContexto = _context.Detalles.Include(d => d.Pedido).Include(d => d.Producto);
            return View(await mvcTiendaContexto.ToListAsync());
        }

        // GET: Detalles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Detalles == null)
            {
                return NotFound();
            }

            var detalle = await _context.Detalles
                .Include(d => d.Pedido)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detalle == null)
            {
                return NotFound();
            }

            return View(detalle);
        }

        // GET: Detalles/Create
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

            ViewData["PedidoId"] = cliente.Id; // Asignar el Id del cliente al ViewData
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion");
            return View();
        }

        // POST: Detalles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Detalles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PedidoId,ProductoId,Cantidad,Precio,Descuento")] Detalle detalle)
        {
            // Obtener el cliente correspondiente al usuario actual
            var userEmail = User.Identity.Name;
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);

            if (cliente != null)
            {
                detalle.PedidoId = cliente.Id; // Asignar el Id del cliente al PedidoId del detalle
            }

            if (ModelState.IsValid)
            {
                _context.Add(detalle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "Id", detalle.PedidoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", detalle.ProductoId);
            return View(detalle);
        }

        // GET: Detalles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Detalles == null)
            {
                return NotFound();
            }

            var detalle = await _context.Detalles.FindAsync(id);
            if (detalle == null)
            {
                return NotFound();
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "Id", detalle.PedidoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", detalle.ProductoId);
            return View(detalle);
        }

        // POST: Detalles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PedidoId,ProductoId,Cantidad,Precio,Descuento")] Detalle detalle)
        {
            if (id != detalle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detalle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetalleExists(detalle.Id))
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
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "Id", detalle.PedidoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", detalle.ProductoId);
            return View(detalle);
        }

        // GET: Detalles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Detalles == null)
            {
                return NotFound();
            }

            var detalle = await _context.Detalles
                .Include(d => d.Pedido)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detalle == null)
            {
                return NotFound();
            }

            return View(detalle);
        }

        // POST: Detalles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Detalles == null)
            {
                return Problem("Entity set 'MvcTiendaContexto.Detalles'  is null.");
            }
            var detalle = await _context.Detalles.FindAsync(id);
            if (detalle != null)
            {
                _context.Detalles.Remove(detalle);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetalleExists(int id)
        {
          return (_context.Detalles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
