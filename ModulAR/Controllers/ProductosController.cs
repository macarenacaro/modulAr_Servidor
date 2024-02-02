using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModulAR.Data;
using ModulAR.Models;
using X.PagedList;

namespace ModulAR.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ProductosController : Controller
    {
        private readonly MvcTiendaContexto _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductosController(MvcTiendaContexto context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Productos
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            // Filtro de búsqueda
            ViewData["CurrentFilter"] = searchString;

            var productos = from p in _context.Productos.Include(p => p.Categoria)
                            select p;

            // Aplicar el filtro de búsqueda
            if (!String.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(p =>
                    p.Descripcion.Contains(searchString) ||
                    p.Id.ToString().Contains(searchString) ||
                    p.PrecioCadena.Contains(searchString)
                );
            }

            // Paginación
            int pageSize = 7; // Número de productos por página
            int pageNumber = (page ?? 1);

            // Devolver la vista con la lista paginada de productos
            return View(await productos.ToPagedListAsync(pageNumber, pageSize));
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Descripcion");
            return View();
        }


        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,Texto,Precio,PrecioCadena,Stock,Escaparate,Imagen,CategoriaId")] Producto producto, IFormFile imagen)
        {
            if (ModelState.IsValid)
            {
                // Verificar si se ha proporcionado una imagen
                if (imagen != null && imagen.Length > 0)
                {
                    // Copiar archivo de imagen
                    string strRutaImagenes = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes");
                    string strExtension = Path.GetExtension(imagen.FileName);
                    string strNombreFichero = Guid.NewGuid().ToString() + strExtension;
                    string strRutaFichero = Path.Combine(strRutaImagenes, strNombreFichero);
                    using (var fileStream = new FileStream(strRutaFichero, FileMode.Create))
                    {
                        imagen.CopyTo(fileStream);
                    }

                    // Asignar el nombre del archivo al producto
                    producto.Imagen = strNombreFichero;
                }

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Descripcion", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Descripcion", producto.CategoriaId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,Texto,Precio,PrecioCadena,Stock,Escaparate,CategoriaId")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Recuperar el producto original de la base de datos con la imagen actual
                    var originalProducto = await _context.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

                    // Restaurar la propiedad Imagen del producto original al modelo antes de actualizar
                    producto.Imagen = originalProducto.Imagen;

                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
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
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Descripcion", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Productos == null)
            {
                return Problem("Entity set 'MvcTiendaContexto.Productos'  is null.");
            }
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Productos/CambiarImagen/5
        public async Task<IActionResult> CambiarImagen(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }
            var producto = await _context.Productos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }


        // POST: Productos/CambiarImagen/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarImagen(int? id, IFormFile imagen)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            // Actualizar producto solo si la imagen es válida
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
                {
                    // Copiar archivo de imagen
                    string strRutaImagenes = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes");
                    string strExtension = Path.GetExtension(imagen.FileName);
                    string strNombreFichero = producto.Id.ToString() + strExtension;
                    string strRutaFichero = Path.Combine(strRutaImagenes, strNombreFichero);

                    using (var fileStream = new FileStream(strRutaFichero, FileMode.Create))
                    {
                        imagen.CopyTo(fileStream);
                    }

                    // Actualizar producto con nueva imagen
                    producto.Imagen = strNombreFichero;

                    try
                    {
                        _context.Update(producto);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProductoExists(producto.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    // Agregar error de modelo si no se proporcionó una imagen
                    ModelState.AddModelError("Imagen", "La imagen es requerida.");
                }
            }

            return View(producto);
        }



        private bool ProductoExists(int id)
        {
          return (_context.Productos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
