using K_Gest.BancoDados;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using K_Gest.Models;

namespace K_Gest.Controllers
{
    public class InsumosController : Controller
    {
        //-----------------------------------------------------------
        // SELECIONAR
        //-----------------------------------------------------------
        public IActionResult Selecionar()
        {
            try
            {
                Insumos o_Insumos = new Insumos();
                DataTable dtInsumos = o_Insumos.SelecionarTodos();
                return View("SelecionarView", dtInsumos);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return View("SelecionarView");
            }
        }

        public IActionResult InserirExibir()
        {
            return View("InserirExibirView");
        }

        //-----------------------------------------------------------
        // INSERIR - PROCESSAR
        //-----------------------------------------------------------
        public IActionResult InserirProcessar(InsumosViewModel o_InsumosVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Insumos o_Insumos = new Insumos();

                    o_Insumos.nomeInsumo = o_InsumosVM.NomeInsumo;
                    o_Insumos.unidadeMed = o_InsumosVM.UnidadeMed;
                    o_Insumos.estoqueAtual = o_InsumosVM.EstoqueAtual;
                    o_Insumos.pontoPedido = o_InsumosVM.PontoPedido;

                    o_Insumos.Inserir();

                    
                    TempData["MsgSucesso"] = "Insumo inserido com sucesso!";

                    return RedirectToAction("Selecionar");
                }

                return View("InserirExibirView", o_InsumosVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return View("InserirExibirView", o_InsumosVM);
            }
        }

        //-----------------------------------------------------------
        // ALTERAR - EXIBIR
        //-----------------------------------------------------------


        public IActionResult AlterarExibir(int idInsumo)
        {
            try
            {
                Insumos o_Insumos = new Insumos();
                o_Insumos.idInsumo = idInsumo;
                DataTable pesqInsumo = o_Insumos.SelecionarPorID();

                //Verificar se retornou dados antes de acessar as Rows
                if (pesqInsumo == null || pesqInsumo.Rows.Count == 0)
                    return RedirectToAction("Selecionar");

                InsumosViewModel o_InsumosVM = new InsumosViewModel();

                
                o_InsumosVM.IdInsumo = Convert.ToInt32(pesqInsumo.Rows[0]["idInsumo"]);
                o_InsumosVM.NomeInsumo = pesqInsumo.Rows[0]["nomeInsumo"].ToString();
                o_InsumosVM.UnidadeMed = pesqInsumo.Rows[0]["unidadeMed"].ToString();
                o_InsumosVM.PontoPedido = Convert.ToDecimal(pesqInsumo.Rows[0]["pontoPedido"]);
                o_InsumosVM.EstoqueAtual = Convert.ToDecimal(pesqInsumo.Rows[0]["estoqueAtual"]);

                return View("AlterarExibirView", o_InsumosVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return RedirectToAction("Selecionar");
            }
        }

        //-----------------------------------------------------------
        // ALTERAR - PROCESSAR
        //-----------------------------------------------------------
        public IActionResult AlterarProcessar(InsumosViewModel o_InsumosVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Insumos o_Insumos = new Insumos();

                    o_Insumos.idInsumo = o_InsumosVM.IdInsumo;
                    o_Insumos.nomeInsumo = o_InsumosVM.NomeInsumo;
                    o_Insumos.unidadeMed = o_InsumosVM.UnidadeMed;
                    o_Insumos.estoqueAtual = o_InsumosVM.EstoqueAtual;
                    o_Insumos.pontoPedido = o_InsumosVM.PontoPedido;

                    o_Insumos.Alterar();

                    TempData["MsgSucesso"] = "Insumo alterado com sucesso!";

                    return RedirectToAction("Selecionar");
                }
                return View("AlterarExibirView", o_InsumosVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return View("AlterarExibirView", o_InsumosVM);
            }
        }

        
        public IActionResult ExcluirExibir(int idInsumo)
        {
            try
            {
                Insumos o_Insumos = new Insumos();
                o_Insumos.idInsumo = idInsumo;
                DataTable pesqInsumo = o_Insumos.SelecionarPorID();

                if (pesqInsumo == null || pesqInsumo.Rows.Count == 0)
                    return RedirectToAction("Selecionar");

                InsumosViewModel o_InsumosVM = new InsumosViewModel();

                o_InsumosVM.IdInsumo = Convert.ToInt32(pesqInsumo.Rows[0]["idInsumo"]);
                o_InsumosVM.NomeInsumo = pesqInsumo.Rows[0]["nomeInsumo"].ToString();
                o_InsumosVM.UnidadeMed = pesqInsumo.Rows[0]["unidadeMed"].ToString();
                o_InsumosVM.PontoPedido = Convert.ToDecimal(pesqInsumo.Rows[0]["pontoPedido"]);
                o_InsumosVM.EstoqueAtual = Convert.ToDecimal(pesqInsumo.Rows[0]["estoqueAtual"]);

                return View("ExcluirExibirView", o_InsumosVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao carregar dados: {ex.Message}";
                return RedirectToAction("Selecionar");
            }
        }

        //-----------------------------------------------------------
        // EXCLUIR - PROCESSAR
        //-----------------------------------------------------------
        public IActionResult ExcluirProcessar(InsumosViewModel o_InsumosVM)
        {
            try
            {
                Insumos o_Insumos = new Insumos();
                o_Insumos.idInsumo = o_InsumosVM.IdInsumo;
                o_Insumos.Excluir();

                TempData["MsgSucesso"] = "Insumo excluído com sucesso!";
                return RedirectToAction("Selecionar");
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Não foi possível excluir: {ex.Message}";
                return View("ExcluirExibirView", o_InsumosVM);
            }
        }
    }
}