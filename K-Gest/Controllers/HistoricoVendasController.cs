using System;
using System.Data;
using K_Gest.BancoDados;
using K_Gest.Models;
using Microsoft.AspNetCore.Mvc;

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
                HistoricoVendas o_HistoricoVendas = new HistoricoVendas();

                DataTable dtVendas = o_HistoricoVendas.SelecionarTodos();

                return View("SelecionarView", dtVendas);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";

                return View("SelecionarView");
            }
        }

        //-----------------------------------------------------------
        // INSERIR - EXIBIR
        //----------------------------------------------------------- 
        public IActionResult InserirExibir()
        {
            return View("InserirExibirView");
        }

        //-----------------------------------------------------------
        // INSERIR - PROCESSAR
        //-----------------------------------------------------------
        public IActionResult InserirProcessar(HistoricoVendasViewModel o_HistoricoVendasVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HistoricoVendas o_HistoricoVendas = new HistoricoVendas();

                    o_HistoricoVendas.dataVend = o_HistoricoVendasVM.DataVend;
                    o_HistoricoVendas.qtdVendida = o_HistoricoVendasVM.QtdVendida;
                    o_HistoricoVendas.idReceita = o_HistoricoVendasVM.IdReceita;
                    o_HistoricoVendas.Inserir();

                    TempData["MsgSucesso"] = "Histórico de vendas inserido com sucesso!";

                    return RedirectToAction("Selecionar");
                }

                return View("InserirExibirView", o_HistoricoVendasVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return View("InserirExibirView", o_HistoricoVendasVM);
            }
        }

        //-----------------------------------------------------------
        // ALTERAR - EXIBIR
        //-----------------------------------------------------------
        public IActionResult AlterarExibir(int idVendas)
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

                return View("AlterarExibirView", o_HistoricoVendasVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return View("AlterarExibirView");
            }
        }

        //-----------------------------------------------------------
        // ALTERAR - PROCESSAR
        //-----------------------------------------------------------
        public IActionResult AlterarProcessar(HistoricoVendasViewModel o_HistoricoVendasVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HistoricoVendas o_HistoricoVendas = new HistoricoVendas();

                    o_HistoricoVendas.idVendas = o_HistoricoVendasVM.IdVendas;
                    o_HistoricoVendas.dataVend = o_HistoricoVendasVM.DataVend;
                    o_HistoricoVendas.qtdVendida = o_HistoricoVendasVM.QtdVendida;
                    o_HistoricoVendas.idReceita = o_HistoricoVendasVM.IdReceita;
                    o_HistoricoVendas.Alterar();

                    TempData["MsgSucesso"] = "Histórico de vendas alterado com sucesso!";

                    return RedirectToAction("Selecionar");
                }
                return View("AlterarExibirView", o_HistoricoVendasVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return View("AlterarExibirView", o_HistoricoVendasVM);
            }
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
