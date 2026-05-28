using System;
using Microsoft.AspNetCore.Mvc;
using K_Gest.Models;
using K_Gest.BancoDados;
using System.Data; // Necessário para trabalhar com o DataTable da Query

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

        // --- A NOVA ACTION DO SEU BOTÃO ---
        [HttpGet]
        public IActionResult GerarListaCompras()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                // Instanciamos a classe de banco (podemos criar o método lá dentro de 'Inicio' ou criar uma classe 'Estoque' depois)
                Inicio bd = new Inicio();

                // O método executa aquela nossa Query híbrida com os JOINs e nos traz um DataTable pronto
                DataTable dtLista = bd.ObterListaCompras();

                // Retorna para uma nova View passando o DataTable contendo os insumos faltantes
                return View("ListaComprasView", dtLista);
            }
            catch (Exception ex)
            {
                TempData["MsgErroLista"] = "Não foi possível gerar a lista de previsão: " + ex.Message;
                return RedirectToAction("IndexView");
            }
        }
    }
}