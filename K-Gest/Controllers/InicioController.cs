using System;
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
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                Inicio bd = new Inicio();
                DashboardViewModel dashboardDados = bd.CarregarDadosDashboard();
                return View("IndexView", dashboardDados);
            }
            catch (Exception ex)
            {
                ViewBag.ErroDashboard = "Erro ao carregar o painel: " + ex.Message;
                return View("IndexView", new DashboardViewModel());
            }
        }
    }
}