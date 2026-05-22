using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using K_Gest.Models;
using K_Gest.BancoDados;

namespace K_Gest.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // ALTERADO: Redireciona para o controlador "Inicio" ao invés de "Home"
                return RedirectToAction("IndexView", "Inicio");
            }

            return View("IndexView");
        }

        [HttpPost]
        public async Task<IActionResult> Autenticar(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("IndexView", vm);
            }

            try
            {
                Usuarios bd = new Usuarios();
                var usuarioValido = bd.ValidarAcesso(vm.Usuario!, vm.Senha!);

                if (usuarioValido != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuarioValido.NomeExibicao!),
                        new Claim(ClaimTypes.Role, usuarioValido.Perfil!),
                        new Claim("LoginUsuario", usuarioValido.Usuario!)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    // ALTERADO: Redireciona para o painel moderno em "Inicio"
                    return RedirectToAction("IndexView", "Inicio");
                }

                ViewBag.Erro = "Usuário ou senha inválidos.";
                return View("IndexView", vm);
            }
            catch (Exception ex)
            {
                ViewBag.Erro = "Erro interno: " + ex.Message;
                return View("IndexView", vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Sair()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
    }
}