using Microsoft.AspNetCore.Mvc;
using K_Gest.Models;
using K_Gest.BancoDados;

namespace K_Gest.Controllers
{
    public class InicioController : Controller
    {
        [HttpGet]
        public IActionResult IndexView()
        {
            // Validação de Segurança: Se não estiver logado, barra e manda pro login
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                // Chamando a classe atualizada sem o sufixo "BD"
                Inicio bd = new Inicio();
                DashboardViewModel dashboardDados = bd.CarregarDadosDashboard();

                return View("IndexView", dashboardDados);
            }
            catch (Exception ex)
            {
                // Repassa a mensagem para a tela caso haja falhas na query ou conexão
                ViewBag.ErroDashboard = ex.Message;
                return View("IndexView", new DashboardViewModel());
            }
        }
    }
}