using K_Gest.BancoDados;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using K_Gest.Models;


namespace K_Gest.Controllers
{
    public class ReceitasController : Controller
    {

        //-----------------------------------------------------------
        // SELECIONAR
        //-----------------------------------------------------------
        public IActionResult Selecionar()
        {
            try
            {
                Receitas o_Receitas = new Receitas();

                DataTable dtReceitas = o_Receitas.SelecionarTodos();

                return View("SelecionarView", dtReceitas);
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
        public IActionResult InserirProcessar(ReceitasViewModel o_ReceitasVM)
        {
            try
            {
                // Se os campos forem validados entra aqui
                if (ModelState.IsValid)
                {
                    Receitas o_Receitas = new Receitas();

                    //Passando os valores que estão na model para a classe que insere no Banco de Dados
                    o_Receitas.nomePrato = o_ReceitasVM.NomePrato;

                    o_Receitas.Inserir();

                    TempData["MsgSucesso"] = "Receita inserida com sucesso!";

                    return RedirectToAction("Selecionar");
                }

                return View("InserirExibirView", o_ReceitasVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return View("InserirExibirView", o_ReceitasVM);
            }
        }


        //-----------------------------------------------------------
        // ALTERAR - EXIBIR
        //-----------------------------------------------------------
        public IActionResult AlterarExibir(int idReceita)
        {
            try
            {
                //--------------------------------------------------
                // Buscar dados do Receitas no banco de dados
                //--------------------------------------------------
                Receitas o_Receitas = new Receitas();

                o_Receitas.idReceita = idReceita;
                DataTable pesqReceitas = o_Receitas.SelecionarPorID();

                //--------------------------------------------------
                // Preencher a Model com os dados do Banco de Dados
                //--------------------------------------------------
                ReceitasViewModel o_ReceitasVM = new ReceitasViewModel();

                //Campos que não podem ser nulos
                o_ReceitasVM.IdReceita = idReceita;
                o_ReceitasVM.NomePrato = pesqReceitas.Rows[0]["NomePrato"].ToString();

               

                return View("AlterarExibirView", o_ReceitasVM);
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
        public IActionResult AlterarProcessar(ReceitasViewModel o_ReceitasVM)
        {
            try
            {
                // Se os campos forem validados entra aqui
                if (ModelState.IsValid)
                {
                    Receitas o_Receitas = new Receitas();

                    //Passando os valores que estão na model para a classe que insere no Banco de Dados
                    o_Receitas.idReceita = o_ReceitasVM.IdReceita;
                    o_Receitas.nomePrato = o_ReceitasVM.NomePrato;
                 
                    o_Receitas.Alterar();

                    TempData["MsgSucesso"] = "Receita alterada com sucesso!";

                    return RedirectToAction("Selecionar");
                }
                return View("AlterarExibirView", o_ReceitasVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return View("AlterarExibirView", o_ReceitasVM);
            }

        }


        //-----------------------------------------------------------
        // EXCLUIR - EXIBIR
        //----------------------------------------------------------- 
        public IActionResult ExcluirExibir(int idReceita)
        {
            try
            {
                //--------------------------------------------------
                // Buscar dados do Receitas no banco de dados
                //--------------------------------------------------
                Receitas o_Receitas = new Receitas();

                o_Receitas.idReceita = idReceita;
                DataTable pesqReceitas = o_Receitas.SelecionarPorID();

                //--------------------------------------------------
                // Preencher a Model com os dados do Banco de Dados
                //--------------------------------------------------
                ReceitasViewModel o_ReceitasVM = new ReceitasViewModel();

                //Campos que não podem ser nulos
                o_ReceitasVM.IdReceita = idReceita;
                o_ReceitasVM.NomePrato = pesqReceitas.Rows[0]["NomePrato"].ToString();

                

                return View("ExcluirExibirView", o_ReceitasVM);
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
        public IActionResult ExcluirProcessar(ReceitasViewModel o_ReceitasVM)
        {
            try
            {
                Receitas o_Receitas = new Receitas();
                o_Receitas.idReceita = o_ReceitasVM.IdReceita;

                // 🔍 Verificar se existem máquinas vinculadas
                //if (o_Receitas.PossuiMaquinasVinculadas())
                //{
                //    TempData["MsgErro"] = "Não é possível excluir este setor pois existem máquinas vinculadas a ele.";
                //    return RedirectToAction("Selecionar");
                //}

                o_Receitas.Excluir();

                TempData["MsgSucesso"] = "Setor excluído com sucesso!";
                return RedirectToAction("Selecionar");
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return View("ExcluirExibirView", o_ReceitasVM);
            }
        }
    }
}
