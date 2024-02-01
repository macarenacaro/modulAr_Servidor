using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModulAR.Data;
using ModulAR.Models;
using System.Data;
using ModulAR.Areas.Identity.Pages.Account;
using X.PagedList;

namespace ModulAR.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public UsuariosController(ApplicationDbContext context,
UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index(string searchString, int? page)
        {
            ViewData["CurrentFilter"] = searchString;

            var usuarios = from user in _context.Users
                           join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                           join role in _context.Roles on userRoles.RoleId equals role.Id
                           select new ViewUsuarioConRol
                           {
                               Email = user.Email,
                               NombreUsuario = user.UserName,
                               RolDeUsuario = role.Name
                           };

            if (!string.IsNullOrEmpty(searchString))
            {
                usuarios = usuarios.Where(u =>
                    u.Email.Contains(searchString) ||
                    u.NombreUsuario.Contains(searchString) ||
                    u.RolDeUsuario.Contains(searchString)
                );
            }

            int pageSize = 7; // ajusta el tamaño de la página según tus necesidades
            int pageNumber = page ?? 1;

            return View(usuarios.ToPagedList(pageNumber, pageSize));
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,Password")]
RegisterModel.InputModel model)
        {
            // Se crea el nuevo usuario
            var user = new IdentityUser();
            user.UserName = model.Email;
            user.Email = model.Email;
            string usuarioPWD = model.Password;
            var result = await _userManager.CreateAsync(user, usuarioPWD);
            // Se asigna el rol de "Administrador" al usuario
            if (result.Succeeded)
            {
                var result1 = await _userManager.AddToRoleAsync(user, "Administrador");
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }






    }
}
