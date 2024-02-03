using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModulAR.Data;
using ModulAR.Models;
using System.Diagnostics;


namespace ModulAR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MvcTiendaContexto _context;

        public HomeController(ILogger<HomeController> logger, MvcTiendaContexto context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Busca el cliente correspondiente al usuario actual. Si existe, activa la
            // vista (View) y en caso contrario, se redirige para crear el cliente.
            string? emailUsuario = User.Identity.Name;
            Cliente cliente = _context.Clientes.SingleOrDefault(c => c.Email == emailUsuario);

            if (User.Identity.IsAuthenticated && User.IsInRole("Usuario") && cliente == null)
            {
                return RedirectToAction("Create", "MisDatos");
            }

			var productos = _context.Productos.Take(8).ToList();

			// Pasar la lista de productos a la vista
			return View(productos);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            Console.WriteLine("Cerrando sesión del usuario...");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Limpiar la variable de sesión NumPedido al cerrar sesión
            HttpContext.Session.Remove("NumPedido");

            Console.WriteLine("Sesión cerrada correctamente.");

            return RedirectToAction(nameof(Index));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Privacy()
        {
             return View();
        }


    }
}