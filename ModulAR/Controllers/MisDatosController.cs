using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModulAR.Data;
using ModulAR.Models;
using System.Data;
using System.Security.Claims;

namespace ModulAR.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class MisDatosController : Controller
    {
        private readonly MvcTiendaContexto _context;
        public MisDatosController(MvcTiendaContexto context)
        {
            _context = context;
        }
        // GET: MisDatos/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: MisDatos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Email,Telefono,Direccion,Poblacion,CodidoPostal,Nif")] Cliente cliente)
        {
            // Asignar el Email y el Nombre del usuario actual
            cliente.Email = User.Identity.Name;

            // Obtener el nombre del usuario actual y asignarlo al campo Nombre del cliente
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.Name);
            if (claim != null)
            {
                cliente.Nombre = claim.Value;
            }

            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(cliente);
        }

        //codigo de edicion añadido
        // GET: MisDatos/Edit
        public async Task<IActionResult> Edit()
        {
            // Se seleccionan los datos del cliente correspondiente al usuario actual
            string? emailUsuario = User.Identity.Name;
            Cliente? cliente = await _context.Clientes
                .Where(c => c.Email == emailUsuario)
                .FirstOrDefaultAsync();

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: MisDatos/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Nombre,Email,Telefono,Direccion,Poblacion,CodigoPostal,Nif")] Cliente cliente)
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

                return RedirectToAction("Index", "Home");
            }

            return View(cliente);
        }

        private bool ClienteExists(int id)
        {
            return (_context.Clientes?.Any(c => c.Id == id)).GetValueOrDefault();
        }




    }
}
