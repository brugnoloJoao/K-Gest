using K_Gest.BancoDados;
using K_Gest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Data;

namespace K_Gest.Controllers
{
    public class HistoricoVendasController : Controller
    {
        //-----------------------------------------------------------
        // SELECIONAR
        //-----------------------------------------------------------
        public IActionResult Selecionar()
        {
            try
            {
                var receitas = new Receitas().SelecionarTodos();

                if (receitas.Rows.Count == 0 || receitas == null)
                {
                    ViewBag.SemReceitas = true; // Indica que não há receitas cadastradas
                }
                DataTable dtVendas = new HistoricoVendas().SelecionarTodos();

                return View("SelecionarView", dtVendas);
            }
            catch (Exception ex) { TempData["MsgErro"] = ex.Message; return View("SelecionarView"); }
        }

        public IActionResult InserirExibir()
        {
            HistoricoVendasViewModel o_HistoricoVendasVM = new HistoricoVendasViewModel();
            o_HistoricoVendasVM.ListaReceitas = ObterReceitas();
            return View("InserirExibirView", o_HistoricoVendasVM);
        }

        [HttpPost]
        public IActionResult InserirProcessar(HistoricoVendasViewModel o_HistoricoVendasVM)
        {
            ModelState.Remove("ListaVendas");// Remover a validação para a propriedade ListaVendas
            if (ModelState.IsValid)
            {
                try
                {
                    HistoricoVendas o_HistoricoVendas = new HistoricoVendas();
                    o_HistoricoVendas.dataVend = o_HistoricoVendasVM.DataVend;
                    o_HistoricoVendas.qtdVendida = o_HistoricoVendasVM.QtdVendida;
                    o_HistoricoVendas.idReceita = o_HistoricoVendasVM.IdReceita;

                    o_HistoricoVendas.Inserir(); // Aciona a transação de venda + estoque

                    TempData["MsgSucesso"] = "Venda registada e estoque atualizado!";
                    return RedirectToAction("Selecionar");
                }
                catch (Exception ex) { TempData["MsgErro"] = ex.Message; }
            }
            ViewBag.ListaReceitas = ObterReceitas();
            return View("InserirExibirView", o_HistoricoVendasVM);
        }

        private List<SelectListItem> ObterReceitas()
        {
            // Substituir pelo método de busca da sua classe Receitas
            DataTable dt = new Receitas().SelecionarTodos();
            return (from DataRow dr in dt.Rows
                    select new SelectListItem
                    {
                        Value = dr["idReceita"].ToString(),
                        Text = dr["nomePrato"].ToString()
                    }).ToList();
        }

        //-----------------------------------------------------------
        // EXCLUIR - EXIBIR
        //----------------------------------------------------------- 
        public IActionResult ExcluirExibir(int idVendas)
        {
            try
            {
                //--------------------------------------------------
                // Buscar dados do HistoricoVendas no banco de dados
                //--------------------------------------------------
                HistoricoVendas o_HistoricoVendas = new HistoricoVendas();

                o_HistoricoVendas.idVendas = idVendas;
                DataTable pesqVendas = o_HistoricoVendas.SelecionarPorID();

                //--------------------------------------------------
                // Preencher a Model com os dados do Banco de Dados
                //--------------------------------------------------
                HistoricoVendasViewModel o_HistoricoVendasVM = new HistoricoVendasViewModel();

                // Campos que não podem ser nulos
                o_HistoricoVendasVM.IdVendas = idVendas;
                o_HistoricoVendasVM.DataVend = Convert.ToDateTime(pesqVendas.Rows[0]["DataVend"]);
                o_HistoricoVendasVM.QtdVendida = Convert.ToInt32(pesqVendas.Rows[0]["QtdVendida"]);
                o_HistoricoVendasVM.IdReceita = Convert.ToInt32(pesqVendas.Rows[0]["IdReceita"]);

                o_HistoricoVendasVM.ListaReceitas = ObterReceitas();

                return View("ExcluirExibirView", o_HistoricoVendasVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return View("ExcluirExibirView");
            }
        }

        //-----------------------------------------------------------
        // EXCLUIR - PROCESSAR
        //-----------------------------------------------------------
        public IActionResult ExcluirProcessar(HistoricoVendasViewModel o_HistoricoVendasVM)
        {
            try
            {
                HistoricoVendas o_HistoricoVendas = new HistoricoVendas();
                o_HistoricoVendas.idVendas = o_HistoricoVendasVM.IdVendas;

                o_HistoricoVendas.Excluir();

                TempData["MsgSucesso"] = "Venda excluída com sucesso!";
                return RedirectToAction("Selecionar");
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return View("ExcluirExibirView", o_HistoricoVendasVM);
            }
        }
    }
}
