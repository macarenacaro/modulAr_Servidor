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
    public class ClientesController : Controller
    {
        private readonly MvcTiendaContexto _context;

        public ClientesController(MvcTiendaContexto context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            var clientes = from c in _context.Clientes select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                clientes = clientes.Where(c =>
                    c.Nombre.Contains(searchString) ||
                    c.Email.Contains(searchString) ||
                    c.Direccion.Contains(searchString) ||
                    c.Nif.Contains(searchString) ||
                    c.Id.ToString().Contains(searchString)
                );
            }

            int pageSize = 7; // ajusta el tamaño de la página según tus necesidades
            int pageNumber = page ?? 1;

            return View(await clientes.ToPagedListAsync(pageNumber, pageSize));
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Email,Telefono,Direccion,Poblacion,CodigoPostal,Nif")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Email,Telefono,Direccion,Poblacion,CodigoPostal,Nif")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
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
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            // Verificar si el cliente tiene pedidos asociados
            ViewBag.HasOrders = _context.Pedidos.Any(p => p.ClienteId == id);

            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clientes == null)
            {
                return Problem("Entity set 'MvcTiendaContexto.Clientes' is null.");
            }

            var cliente = await _context.Clientes.FindAsync(id);

            // Verificar si el cliente tiene pedidos asociados
            if (_context.Pedidos.Any(p => p.ClienteId == id))
            {
                ModelState.AddModelError(string.Empty, "No se puede eliminar el cliente porque tiene pedidos asociados.");
                return View(cliente);
            }

            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
          return (_context.Clientes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
