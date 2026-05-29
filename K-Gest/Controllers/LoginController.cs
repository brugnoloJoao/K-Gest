using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using K_Gest.Models;
using K_Gest.BancoDados;

namespace K_Gest.Controllers
{
    public class LoginController : Controller
    {
        // Método privado para gerar o Hash da senha (agora retornando byte[] em vez de string)
        private byte[] GerarHash(string senha)
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(senha));
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
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
                // Agora isso é um array de bytes
                byte[] senhaComHash = GerarHash(vm.Senha!);

                Usuarios bd = new Usuarios();
                // Passa o array de bytes para a validação
                var usuarioValido = bd.ValidarAcesso(vm.Usuario!, senhaComHash);

                if (usuarioValido != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuarioValido.NomeExibicao!),
                        new Claim(ClaimTypes.Role, "Usuario"),
                        new Claim("LoginUsuario", usuarioValido.Usuario!)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

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

        // ==========================================
        // NOVOS MÉTODOS PARA O CADASTRO DE USUÁRIOS
        // ==========================================

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View("CadastroView");
        }

        [HttpPost]
        public async Task<IActionResult> SalvarCadastro(CadastroViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("CadastroView", vm);
            }

            try
            {
                Usuarios bd = new Usuarios();

                if (bd.ExisteUsuario(vm.Usuario!))
                {
                    ViewBag.Erro = "Este usuário já está em uso.";
                    return View("CadastroView", vm);
                }

                // Gera o array de bytes da senha
                byte[] hashDaSenha = GerarHash(vm.Senha!);

                // Salva no Banco de Dados passando o ViewModel e o hash em bytes separados
                bd.Inserir(vm, hashDaSenha);

                // Login Automático
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, vm.NomeExibicao!),
                    new Claim(ClaimTypes.Role, "Usuario"),
                    new Claim("LoginUsuario", vm.Usuario!)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("IndexView", "Inicio");
            }
            catch (Exception ex)
            {
                ViewBag.Erro = "Erro ao realizar cadastro: " + ex.Message;
                return View("CadastroView", vm);
            }
        }
    }
}