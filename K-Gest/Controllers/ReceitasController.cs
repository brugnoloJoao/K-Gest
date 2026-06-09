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
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                Receitas o_Receitas = new Receitas();
                DataTable dtReceitas = o_Receitas.SelecionarTodos();

                
                return View("SelecionarView", dtReceitas ?? new DataTable());
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao carregar receitas: {ex.Message}";
               
                return View("SelecionarView", new DataTable());
            }
        }

        //-----------------------------------------------------------
        // INSERIR - EXIBIR
        //----------------------------------------------------------- 
        public IActionResult InserirExibir()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }
            return View("InserirExibirView");
        }


        //-----------------------------------------------------------
        // INSERIR - PROCESSAR
        //-----------------------------------------------------------
        public IActionResult InserirProcessar(ReceitasViewModel o_ReceitasVM)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                // Se os campos forem validados entra aqui
                if (ModelState.IsValid)
                {
                    Receitas o_Receitas = new Receitas();

                    //Passando os valores que estão na model para a classe que insere no Banco de Dados
                    o_Receitas.nomePrato = o_ReceitasVM.NomePrato;
                    o_Receitas.preco = o_ReceitasVM.Preco;

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
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                Receitas o_Receitas = new Receitas();
                o_Receitas.idReceita = idReceita;
                DataTable pesqReceitas = o_Receitas.SelecionarPorID();

                // Verificação se o registro existe
                if (pesqReceitas == null || pesqReceitas.Rows.Count == 0)
                {
                    TempData["MsgErro"] = "Receita não encontrada.";
                    return RedirectToAction("Selecionar");
                }

                ReceitasViewModel o_ReceitasVM = new ReceitasViewModel();
                o_ReceitasVM.IdReceita = idReceita;
                
                o_ReceitasVM.NomePrato = pesqReceitas.Rows[0]["nomePrato"].ToString();
                o_ReceitasVM.Preco = Convert.ToDecimal(pesqReceitas.Rows[0]["preco"].ToString());

                return View("AlterarExibirView", o_ReceitasVM);
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
        public IActionResult AlterarProcessar(ReceitasViewModel o_ReceitasVM)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                // Se os campos forem validados entra aqui
                if (ModelState.IsValid)
                {
                    Receitas o_Receitas = new Receitas();

                    //Passando os valores que estão na model para a classe que insere no Banco de Dados
                    o_Receitas.idReceita = o_ReceitasVM.IdReceita;
                    o_Receitas.nomePrato = o_ReceitasVM.NomePrato;
                    o_Receitas.preco = o_ReceitasVM.Preco;

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
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                Receitas o_Receitas = new Receitas();
                o_Receitas.idReceita = idReceita;
                DataTable pesqReceitas = o_Receitas.SelecionarPorID();

                // Verificação de existência
                if (pesqReceitas == null || pesqReceitas.Rows.Count == 0)
                    return RedirectToAction("Selecionar");

                ReceitasViewModel o_ReceitasVM = new ReceitasViewModel();
                o_ReceitasVM.IdReceita = idReceita;
                o_ReceitasVM.NomePrato = pesqReceitas.Rows[0]["nomePrato"].ToString();
                o_ReceitasVM.Preco = Convert.ToDecimal(pesqReceitas.Rows[0]["preco"].ToString());


                return View("ExcluirExibirView", o_ReceitasVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return RedirectToAction("Selecionar");
            }
        }

        //-----------------------------------------------------------
        // EXCLUIR - PROCESSAR
        //-----------------------------------------------------------
        public IActionResult ExcluirProcessar(ReceitasViewModel o_ReceitasVM)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                Receitas o_Receitas = new Receitas();
                o_Receitas.idReceita = o_ReceitasVM.IdReceita;

                o_Receitas.Excluir();

                TempData["MsgSucesso"] = "Receita excluída com sucesso!";
                return RedirectToAction("Selecionar");
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"{ex.Message}";
                return RedirectToAction("Selecionar");
            }
        }
    }
}
