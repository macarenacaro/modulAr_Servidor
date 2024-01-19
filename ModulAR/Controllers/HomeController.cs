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

            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        


    }
}