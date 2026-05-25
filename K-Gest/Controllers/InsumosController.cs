using K_Gest.BancoDados;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using K_Gest.Models;

namespace K_Gest.Controllers
{
    public class InsumosController : Controller
    {
        public IActionResult Selecionar()
        {
            try
            {
                Insumos o_Insumos = new Insumos();
                DataTable dtInsumos = o_Insumos.SelecionarTodos();
                foreach (DataRow row in dtInsumos.Rows)
                {
                    row["estoqueAtual"] = o_Insumos.ConverterParaTela(Convert.ToDecimal(row["EstoqueAtual"]), row["UnidadeMed"].ToString());
                    row["PontoPedido"] = o_Insumos.ConverterParaTela(Convert.ToDecimal(row["PontoPedido"]), row["UnidadeMed"].ToString());
                }
                return View("SelecionarView", dtInsumos);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return View("SelecionarView");
            }
        }

        public IActionResult InserirExibir() => View("InserirExibirView");

        [HttpPost]
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
                TempData["MsgErro"] = ex.Message;
                return View("InserirExibirView", o_InsumosVM);
            }
        }

        public IActionResult AlterarExibir(int idInsumo) 
        {
            Insumos o_Insumos = new Insumos();
            o_Insumos.idInsumo = idInsumo;
            DataTable dt = o_Insumos.SelecionarPorID();

            if (dt != null && dt.Rows.Count > 0)
            {
                InsumosViewModel vm = new InsumosViewModel();
                vm.IdInsumo = Convert.ToInt32(dt.Rows[0]["IdInsumo"]);
                vm.NomeInsumo = dt.Rows[0]["NomeInsumo"].ToString();
                vm.UnidadeMed = dt.Rows[0]["UnidadeMed"].ToString();

                // AQUI ESTÁ O SEGREDO: Converte o valor bruto do banco para o valor de exibição
                decimal estoqueBruto = Convert.ToDecimal(dt.Rows[0]["EstoqueAtual"]);
                decimal pontoBruto = Convert.ToDecimal(dt.Rows[0]["PontoPedido"]);

                vm.EstoqueAtual = o_Insumos.ConverterParaTela(estoqueBruto, vm.UnidadeMed);
                vm.PontoPedido = o_Insumos.ConverterParaTela(pontoBruto, vm.UnidadeMed);

                return View("AlterarExibirView", vm);
            }
            return RedirectToAction("Selecionar");
        }

        [HttpPost]
        public IActionResult AlterarProcessar(InsumosViewModel o_InsumosVM)
        {
            try
            {
                // Forçamos a execução se o ID for válido, ignorando erros automáticos de validação
                if (o_InsumosVM.IdInsumo > 0)
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

                TempData["MsgErro"] = "ID do insumo inválido.";
                return View("AlterarExibirView", o_InsumosVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = "Erro no Banco: " + ex.Message;
                return View("AlterarExibirView", o_InsumosVM);
            }
        }

        public IActionResult ExcluirExibir(int idInsumo)
        {
            Insumos o_Insumos = new Insumos();
            o_Insumos.idInsumo = idInsumo;
            DataTable dt = o_Insumos.SelecionarPorID();

            if (dt != null && dt.Rows.Count > 0)
            {
                InsumosViewModel vm = new InsumosViewModel();
                vm.IdInsumo = idInsumo;
                vm.NomeInsumo = dt.Rows[0]["NomeInsumo"].ToString();
                vm.UnidadeMed = dt.Rows[0]["UnidadeMed"].ToString();
                // Exibe formatado também na tela de confirmação de exclusão
                vm.EstoqueAtual = o_Insumos.ConverterParaTela(Convert.ToDecimal(dt.Rows[0]["EstoqueAtual"]), vm.UnidadeMed);
                vm.PontoPedido = o_Insumos.ConverterParaTela(Convert.ToDecimal(dt.Rows[0]["PontoPedido"]), vm.UnidadeMed);

                return View("ExcluirExibirView", vm);
            }
            return RedirectToAction("Selecionar");
        }

        [HttpPost]
        public IActionResult ExcluirProcessar(InsumosViewModel vm)
        {
            try
            {
                Insumos o_Insumos = new Insumos { idInsumo = vm.IdInsumo };
                o_Insumos.Excluir();
                TempData["MsgSucesso"] = "Excluído com sucesso!";
                return RedirectToAction("Selecionar");
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = ex.Message;
                return View("ExcluirExibirView", vm);
            }
        }
    }
}