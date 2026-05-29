using K_Gest.BancoDados;
using K_Gest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;


namespace K_Gest.Controllers
{
    public class MovimentacaoEstoqueController : Controller
    {

        //-----------------------------------------------------------
        // SELECIONAR
        //-----------------------------------------------------------
        public IActionResult Selecionar()
        {
            try
            {
                MovimentacaoEstoque o_MovimentacaoEstoque = new MovimentacaoEstoque();

                DataTable dtMoviment = o_MovimentacaoEstoque.SelecionarTodos();

                foreach (DataRow row in dtMoviment.Rows)
                {
                    row["qtdMoviment"] = o_MovimentacaoEstoque.ConverterParaTela(Convert.ToDecimal(row["qtdMoviment"]), row["unidadeMed"].ToString());
                }

                return View("SelecionarView", dtMoviment);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";

                return View("SelecionarView");
            }
        }
        public IActionResult Entradas()
        {
            try
            {
                MovimentacaoEstoque o_MovimentacaoEstoque = new MovimentacaoEstoque();

                DataTable dtMoviment = o_MovimentacaoEstoque.SelecionarEntradas();

                foreach (DataRow row in dtMoviment.Rows)
                {
                    row["qtdMoviment"] = o_MovimentacaoEstoque.ConverterParaTela(Convert.ToDecimal(row["qtdMoviment"]), row["unidadeMed"].ToString());
                }

                return View("EntradasView", dtMoviment);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";

                return View("SelecionarView");
            }
        }
        public IActionResult Saidas()
        {
            try
            {
                MovimentacaoEstoque o_MovimentacaoEstoque = new MovimentacaoEstoque();

                DataTable dtMoviment = o_MovimentacaoEstoque.SelecionarSaidas();

                foreach (DataRow row in dtMoviment.Rows)
                {
                    row["qtdMoviment"] = o_MovimentacaoEstoque.ConverterParaTela(Convert.ToDecimal(row["qtdMoviment"]), row["unidadeMed"].ToString());
                }

                return View("SaidasView", dtMoviment);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";

                return View("SaidasView");
            }
        }
        //-----------------------------------------------------------
        // INSERIR - EXIBIR
        //----------------------------------------------------------- 
        public IActionResult InserirExibir()
        {
            try
            {
                MovimentacaoEstoqueViewModel o_MovimentVM = new MovimentacaoEstoqueViewModel();

                // Passamos via ViewBag para preencher um <select>
                o_MovimentVM.ListaInsumos = ObterInsumos();

                return View("InserirExibirView", o_MovimentVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao carregar insumos: {ex.Message}";
                return RedirectToAction("Selecionar");
            }
        }
        public IActionResult InserirSaida()
        {
            try
            {
                MovimentacaoEstoqueViewModel o_MovimentVM = new MovimentacaoEstoqueViewModel();

                // Passamos via ViewBag para preencher um <select>
                o_MovimentVM.ListaInsumos = ObterInsumos();

                return View("InserirSaidaView", o_MovimentVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao carregar insumos: {ex.Message}";
                return RedirectToAction("Saida");
            }
        }
        public IActionResult InserirEntrada()
        {
            try
            {
                MovimentacaoEstoqueViewModel o_MovimentacaoEstoqueVM = new MovimentacaoEstoqueViewModel();

                // Passamos via ViewBag para preencher um <select>
                o_MovimentacaoEstoqueVM.ListaInsumos = ObterInsumos();

                return View("InserirEntradaView", o_MovimentacaoEstoqueVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao carregar insumos: {ex.Message}";
                return RedirectToAction("Saida");
            }
        }
        //-----------------------------------------------------------
        // INSERIR - PROCESSAR
        //-----------------------------------------------------------
        [HttpPost]
        public IActionResult InserirProcessar(MovimentacaoEstoqueViewModel o_MovimentacaoEstoqueVM)
        {
            try
            {
                if (ModelState.ContainsKey("ListaInsumos"))
                {
                    ModelState.Remove("ListaInsumos");
                }

                if (ModelState.IsValid)
                {
                    MovimentacaoEstoque o_Movimentacao = new MovimentacaoEstoque();

                    // Preenche os dados básicos
                    o_Movimentacao.tipoEs = o_MovimentacaoEstoqueVM.TipoEs;
                    o_Movimentacao.qtdMoviment = o_MovimentacaoEstoqueVM.QtdMoviment;
                    o_Movimentacao.motivo = o_MovimentacaoEstoqueVM.Motivo;
                    o_Movimentacao.idInsumo = o_MovimentacaoEstoqueVM.IdInsumo;

                    string unidadeMed = ObterUnidadeMedPorIDInsumo(o_Movimentacao.idInsumo);

                    if (o_Movimentacao.tipoEs == "S")
                    {
                        decimal estoqueAtual = ObterEstoqueAtualDeInsumo(o_Movimentacao.idInsumo);

                        if (o_Movimentacao.qtdMoviment > estoqueAtual)
                        {
                            // Se for maior, impede a gravação e avisa o usuário
                            TempData["MsgErro"] = $"Saldo insuficiente! Você tentou retirar {o_Movimentacao.ConverterParaTela(o_Movimentacao.qtdMoviment, unidadeMed)} {unidadeMed}, mas o estoque atual é de apenas {o_Movimentacao.ConverterParaTela(estoqueAtual, unidadeMed)} {unidadeMed}.";

                            // Recarrega as listas da View para o usuário corrigir o valor
                            o_MovimentacaoEstoqueVM.ListaInsumos = ObterInsumos();
                            return View("SaidasView", o_MovimentacaoEstoqueVM);
                        }

                        o_Movimentacao.Inserir(unidadeMed);

                        // Define o sucesso ANTES do redirect
                        TempData["MsgSucesso"] = "Movimentação de saída realizada com sucesso!";
                        return RedirectToAction("Saidas");
                    }
                    else
                    {
                        o_Movimentacao.Inserir(unidadeMed);

                        // Define o sucesso ANTES do redirect
                        TempData["MsgSucesso"] = "Movimentação de entrada realizada com sucesso!";
                        return RedirectToAction("Entradas");
                    }
                }

                // Se o ModelState falhar, recarrega a lista e devolve para a tela de origem
                o_MovimentacaoEstoqueVM.ListaInsumos = ObterInsumos();
                return View("InserirSaida", o_MovimentacaoEstoqueVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = "Erro ao processar movimentação: " + ex.Message;
                o_MovimentacaoEstoqueVM.ListaInsumos = ObterInsumos();
                return View("InserirSaida", o_MovimentacaoEstoqueVM);
            }
        }

        //-----------------------------------------------------------
        // EXCLUIR - EXIBIR
        //----------------------------------------------------------- 
        public IActionResult ExcluirExibir(int idEstoque)
        {
            try
            {
                //--------------------------------------------------
                // Buscar dados do MovimentacaoEstoque no banco de dados
                //--------------------------------------------------
                MovimentacaoEstoque o_MovimentacaoEstoque = new MovimentacaoEstoque();

                o_MovimentacaoEstoque.idEstoque = idEstoque;
                DataTable pesqMovimentacaoEstoque = o_MovimentacaoEstoque.SelecionarPorID();

                //--------------------------------------------------
                // Preencher a Model com os dados do Banco de Dados
                //--------------------------------------------------
                MovimentacaoEstoqueViewModel o_MovimentacaoEstoqueVM = new MovimentacaoEstoqueViewModel();

                //Campos que não podem ser nulos
                o_MovimentacaoEstoqueVM.IdEstoque = idEstoque;
                o_MovimentacaoEstoqueVM.TipoEs = pesqMovimentacaoEstoque.Rows[0]["TipoEs"].ToString();
                o_MovimentacaoEstoqueVM.QtdMoviment = Convert.ToDecimal(pesqMovimentacaoEstoque.Rows[0]["QtdMoviment"]);
                o_MovimentacaoEstoqueVM.Motivo = pesqMovimentacaoEstoque.Rows[0]["Motivo"].ToString();
                o_MovimentacaoEstoqueVM.IdInsumo = Convert.ToInt32(pesqMovimentacaoEstoque.Rows[0]["IdInsumo"]);
                o_MovimentacaoEstoqueVM.DataMoviment = Convert.ToDateTime(pesqMovimentacaoEstoque.Rows[0]["DataMoviment"]);

                // Passamos via ViewBag para preencher um <select>
                o_MovimentacaoEstoqueVM.ListaInsumos = ObterInsumos();

                return View("ExcluirExibirView", o_MovimentacaoEstoqueVM);
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
        public IActionResult ExcluirProcessar(MovimentacaoEstoqueViewModel o_MovimentacaoEstoqueVM)
        {
            try
            {
                MovimentacaoEstoque o_MovimentacaoEstoque = new MovimentacaoEstoque();
                o_MovimentacaoEstoque.idEstoque = o_MovimentacaoEstoqueVM.IdEstoque;

                o_MovimentacaoEstoque.Excluir();

                TempData["MsgSucesso"] = "Movimentação excluída com sucesso!";
                return RedirectToAction("Selecionar");
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return RedirectToAction("Selecionar");
            }
        }

        private List<SelectListItem> ObterInsumos()
        {
            // Nota: Conforme o código anterior, sua classe de dados chama-se 'Insumos'
            DataTable dt = new Insumos().SelecionarTodos();

            if (dt == null) return new List<SelectListItem>();

            return (from DataRow dr in dt.Rows
                    select new SelectListItem
                    {
                        // Adapte a string de coluna se no seu banco de insumos for minúsculo (ex: "idInsumo" / "nomeInsumo")
                        Value = dr["idInsumo"].ToString(),
                        Text = $"{dr["nomeInsumo"]} ({dr["unidadeMed"]})"
                    }).ToList();
        }
        private string ObterUnidadeMedPorIDInsumo(int idInsumo)
        {
            DataTable dt = new Insumos { idInsumo = idInsumo }.SelecionarPorID();
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["unidadeMed"].ToString();
            }
            return string.Empty; // Retorna vazio se não encontrar o insumo
        }
        private decimal ObterEstoqueAtualDeInsumo(int idInsumo)
        {
            DataTable dt = new Insumos { idInsumo = idInsumo }.SelecionarPorID();
            if (dt != null && dt.Rows.Count > 0)
            {
                return Convert.ToDecimal(dt.Rows[0]["estoqueAtual"].ToString());
            }
            return 0;
        }
    }
}
