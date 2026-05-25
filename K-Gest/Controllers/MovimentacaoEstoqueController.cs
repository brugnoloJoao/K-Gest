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
                MovimentacaoEstoqueViewModel o_MovimentVM = new MovimentacaoEstoqueViewModel();

                // Passamos via ViewBag para preencher um <select>
                o_MovimentVM.ListaInsumos = ObterInsumos();

                return View("InserirEntradaView", o_MovimentVM);
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
        public IActionResult InserirProcessar(MovimentacaoEstoqueViewModel o_MovimentacaoVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MovimentacaoEstoque o_Movimentacao = new MovimentacaoEstoque();

                    // Preenche os dados básicos
                    o_Movimentacao.tipoEs = o_MovimentacaoVM.TipoEs;
                    o_Movimentacao.qtdMoviment = o_MovimentacaoVM.QtdMoviment;
                    o_Movimentacao.motivo = o_MovimentacaoVM.Motivo;
                    o_Movimentacao.idInsumo = o_MovimentacaoVM.IdInsumo;

                    // AGORA VOCÊ PASSA A UNIDADE AQUI DENTRO DOS PARÊNTESES
                    // Isso resolve o erro de "nenhum argumento fornecido"
                    o_Movimentacao.Inserir(o_MovimentacaoVM.UnidadeMed);

                    TempData["MsgSucesso"] = "Movimentação realizada!";
                    return RedirectToAction("Selecionar");
                }
                return View("InserirExibir", o_MovimentacaoVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = ex.Message;
                return View("InserirExibir", o_MovimentacaoVM);
            }
        }

        //-----------------------------------------------------------
        // ALTERAR - EXIBIR
        //-----------------------------------------------------------
        //public IActionResult AlterarExibir(int idEstoque)
        //{
        //    try
        //    {
        //        //--------------------------------------------------
        //        // Buscar dados do MovimentacaoEstoque no banco de dados
        //        //--------------------------------------------------
        //        MovimentacaoEstoque o_MovimentacaoEstoque = new MovimentacaoEstoque();

        //        o_MovimentacaoEstoque.idEstoque = idEstoque;
        //        DataTable pesqSetores = o_MovimentacaoEstoque.SelecionarPorID();

        //        //--------------------------------------------------
        //        // Preencher a Model com os dados do Banco de Dados
        //        //--------------------------------------------------
        //        MovimentacaoEstoqueViewModel o_MovimentacaoEstoqueVM = new MovimentacaoEstoqueViewModel();

        //        //Campos que não podem ser nuloso_MovimentacaoEstoque.tipoEs = o_MovimentacaoEstoqueVM.TipoEs;

        //        o_MovimentacaoEstoqueVM.IdEstoque = idEstoque;
        //        o_MovimentacaoEstoqueVM.TipoEs = pesqSetores.Rows[0]["TipoEs"].ToString();
        //        o_MovimentacaoEstoqueVM.QtdMoviment = Convert.ToInt32(pesqSetores.Rows[0]["QtdMoviment"]);
        //        o_MovimentacaoEstoqueVM.Motivo = pesqSetores.Rows[0]["Motivo"].ToString();
        //        o_MovimentacaoEstoqueVM.IdInsumo = Convert.ToInt32(pesqSetores.Rows[0]["IdInsumo"]);

        //        return View("AlterarExibirView", o_MovimentacaoEstoqueVM);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["MsgErro"] = $"Erro: {ex.Message}";
        //        return View("AlterarExibirView");
        //    }
        //}

        //-----------------------------------------------------------
        // ALTERAR - PROCESSAR
        //-----------------------------------------------------------
        //public IActionResult AlterarProcessar(MovimentacaoEstoqueViewModel o_MovimentacaoEstoqueVM)
        //{
        //    try
        //    {
        //        // Se os campos forem validados entra aqui
        //        if (ModelState.IsValid)
        //        {
        //            MovimentacaoEstoque o_MovimentacaoEstoque = new MovimentacaoEstoque();

        //            //Passando os valores que estão na model para a classe que insere no Banco de Dados
        //            o_MovimentacaoEstoque.idEstoque = o_MovimentacaoEstoqueVM.IdEstoque;
        //            o_MovimentacaoEstoque.tipoEs = o_MovimentacaoEstoqueVM.TipoEs;
        //            o_MovimentacaoEstoque.qtdMoviment = o_MovimentacaoEstoqueVM.QtdMoviment;
        //            o_MovimentacaoEstoque.motivo = o_MovimentacaoEstoqueVM.Motivo;
        //            o_MovimentacaoEstoque.idInsumo = o_MovimentacaoEstoqueVM.IdInsumo;
        //            o_MovimentacaoEstoque.Alterar();

        //            TempData["MsgSucesso"] = "Movimentação de estoque alterada com sucesso!";

        //            return RedirectToAction("Selecionar");
        //        }
        //        return View("AlterarExibirView", o_MovimentacaoEstoqueVM);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["MsgErro"] = $"Erro: {ex.Message}";
        //        return View("AlterarExibirView", o_MovimentacaoEstoqueVM);
        //    }

        //}

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
                DataTable pesqSetores = o_MovimentacaoEstoque.SelecionarPorID();

                //--------------------------------------------------
                // Preencher a Model com os dados do Banco de Dados
                //--------------------------------------------------
                MovimentacaoEstoqueViewModel o_MovimentacaoEstoqueVM = new MovimentacaoEstoqueViewModel();

                //Campos que não podem ser nulos
                o_MovimentacaoEstoqueVM.IdEstoque = idEstoque;
                o_MovimentacaoEstoqueVM.TipoEs = pesqSetores.Rows[0]["TipoEs"].ToString();
                o_MovimentacaoEstoqueVM.QtdMoviment = Convert.ToDecimal(pesqSetores.Rows[0]["QtdMoviment"]);
                o_MovimentacaoEstoqueVM.Motivo = pesqSetores.Rows[0]["Motivo"].ToString();
                o_MovimentacaoEstoqueVM.IdInsumo = Convert.ToInt32(pesqSetores.Rows[0]["IdInsumo"]);

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

                TempData["MsgSucesso"] = "Setor excluído com sucesso!";
                return RedirectToAction("Selecionar");
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return View("ExcluirExibirView", o_MovimentacaoEstoqueVM);
            }
        }
        public IActionResult RelatorioDesperdicio()
        {
            MovimentacaoEstoque o_Movimentacao = new MovimentacaoEstoque();
            // Exemplo de chamada para um método que filtra por motivo no SQL
            DataTable dtDesperdicio = o_Movimentacao.SelecionarPorMotivo("Desperdício");

            return View("DashboardDesperdicio", dtDesperdicio);
        }

        //public IActionResult ListaComprasAutomatica()
        //{
        //    Insumos o_Insumos = new Insumos();
        //    // No SQL: SELECT * FROM Insumos WHERE estoqueAtual <= pontoPedido
        //    DataTable dtParaComprar = o_Insumos.SelecionarAbaixoDoPontoPedido();

        //    return View("ListaComprasView", dtParaComprar);
        //}
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
    }
}
