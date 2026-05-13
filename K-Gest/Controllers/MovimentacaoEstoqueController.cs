using Microsoft.AspNetCore.Mvc;
using System.Data;
using K_Gest.BancoDados;
using K_Gest.Models;


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

                DataTable dtInsumos = o_MovimentacaoEstoque.SelecionarTodos();

                return View("SelecionarView", dtInsumos);
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
            try
            {
                // Precisamos buscar os insumos para o usuário selecionar na View
                Insumos o_Insumos = new Insumos();
                DataTable dtInsumos = o_Insumos.SelecionarTodos();

                // Passamos via ViewBag para preencher um <select>
                ViewBag.ListaInsumos = dtInsumos;

                return View("InserirExibirView");
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao carregar insumos: {ex.Message}";
                return RedirectToAction("Selecionar");
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
                o_MovimentacaoEstoqueVM.QtdMoviment = Convert.ToInt32(pesqSetores.Rows[0]["QtdMoviment"]);
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
    }


}
