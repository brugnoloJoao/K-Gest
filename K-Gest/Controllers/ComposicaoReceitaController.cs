using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using K_Gest.BancoDados;
using K_Gest.Models;
using System.Data;

namespace K_Gest.Controllers
{
    public class ComposicaoReceitaController : Controller
    {
        // --- LISTAGEM ---
        public IActionResult Selecionar()
        {
            ComposicaoReceita o_Comp = new ComposicaoReceita();
            DataTable dt = o_Comp.SelecionarAgrupado();
            return View("SelecionarView", dt);
        }
        [HttpGet]
        public JsonResult ObterIngredientesJson(int id)
        {
            ComposicaoReceita o_Comp = new ComposicaoReceita();
            DataTable dt = o_Comp.SelecionarPorReceita(id);

            var lista = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new
                {
                    // idComp é o que a lixeira do modal vai usar para excluir
                    idComp = row["idComposicao"].ToString(),
                    nomeInsumo = row["nomeInsumo"].ToString(),
                    qtd = row["qtdNecessaria"].ToString(),
                    unidade = row["unidadeMed"].ToString()
                });
            }
            return Json(lista);
        }

        [HttpGet]
        public IActionResult EditarFicha(int id)
        {
            try
            {
                ComposicaoReceita o_Comp = new ComposicaoReceita();
                DataTable dtItens = o_Comp.SelecionarPorReceita(id);

                var vm = new ComposicaoReceitaViewModel
                {
                    IdReceita = id,
                    ListaReceitas = ObterReceitas(),
                    ListaInsumos = ObterInsumos(),
                    Itens = new List<ItemComposicao>()
                };

                foreach (DataRow row in dtItens.Rows)
                {
                    vm.Itens.Add(new ItemComposicao
                    {
                        // Agora idInsumo existe no DataTable devido à correção no SQL
                        IdInsumo = Convert.ToInt32(row["idInsumo"]),
                        NomeInsumo = row["nomeInsumo"].ToString(),
                        Quantidade = Convert.ToDecimal(row["qtdNecessaria"]),
                        UnidadeMed = row["unidadeMed"].ToString()
                    });
                }
                return View("InserirExibirView", vm);
            }
            catch (Exception ex)
            {
                // Exibe o erro na tela caso algo falhe
                return Content("Erro ao carregar: " + ex.Message);
            }
        }

        public IActionResult ExcluirIngrediente(int id)
        {
            new ComposicaoReceita().ExcluirIngredienteIndividual(id);
            return RedirectToAction("Selecionar");
        }

        // --- INSERIR ---
        public IActionResult InserirExibir()
        {
            var vm = new ComposicaoReceitaViewModel
            {
                ListaReceitas = ObterReceitas(),
                ListaInsumos = ObterInsumos()
            };
            return View("InserirExibirView", vm);
        }
        [HttpPost]
        public IActionResult InserirProcessar(ComposicaoReceitaViewModel vm)
        {
            ModelState.Remove("ListaReceitas");
            ModelState.Remove("ListaInsumos");

            // Se a lista "Itens" tiver dados, salvamos todos
            if (vm.IdReceita > 0 && vm.Itens != null && vm.Itens.Count > 0)
            {
                try
                {
                    ComposicaoReceita o_Comp = new ComposicaoReceita();
                    o_Comp.InserirFichaTecnica(vm.IdReceita, vm.Itens);

                    TempData["MsgSucesso"] = "Ficha Técnica salva com sucesso!";
                    return RedirectToAction("Selecionar");
                }
                catch (Exception ex) { TempData["MsgErro"] = ex.Message; }
            }
            else
            {
                TempData["MsgErro"] = "Por favor, selecione a receita e adicione os ingredientes.";
            }

            vm.ListaReceitas = ObterReceitas();
            vm.ListaInsumos = ObterInsumos();
            return View("InserirExibirView", vm);
        }
        // --- ALTERAR ---
        public IActionResult AlterarExibir(int id) // 'id' deve vir da sua tabela na SelecionarView
        {
            try
            {
                ComposicaoReceita o_Comp = new ComposicaoReceita { idComposicao = id };
                DataTable dt = o_Comp.SelecionarPorID();

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    var vm = new ComposicaoReceitaViewModel
                    {
                        IdComposicao = Convert.ToInt32(row["idComposicao"]),
                        IdReceita = Convert.ToInt32(row["idReceita"]),
                        IdInsumo = Convert.ToInt32(row["idInsumo"]),
                        QtdNecessaria = Convert.ToDecimal(row["qtdNecessaria"]),
                        ListaReceitas = ObterReceitas(),
                        ListaInsumos = ObterInsumos()
                    };
                    return View("AlterarExibirView", vm);
                }
            }
            catch (Exception ex) { TempData["MsgErro"] = ex.Message; }
            return RedirectToAction("Selecionar");
        }

        [HttpPost]
        public IActionResult AlterarProcessar(ComposicaoReceitaViewModel vm)
        {
            ModelState.Remove("ListaReceitas");
            ModelState.Remove("ListaInsumos");

            try
            {
                ComposicaoReceita o_Comp = new ComposicaoReceita
                {
                    idComposicao = vm.IdComposicao,
                    idReceita = vm.IdReceita,
                    idInsumo = vm.IdInsumo,
                    qtdNecessaria = vm.QtdNecessaria
                };
                o_Comp.Alterar();
                TempData["MsgSucesso"] = "Alterado com sucesso!";
                return RedirectToAction("Selecionar");
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = ex.Message;
                vm.ListaReceitas = ObterReceitas();
                vm.ListaInsumos = ObterInsumos();
                return View("AlterarExibirView", vm);
            }
        }

        // --- EXCLUIR ---

       
        public IActionResult Excluir(int id)
        {
            try
            {
                ComposicaoReceita o_Comp = new ComposicaoReceita { idComposicao = id };
                o_Comp.Excluir();
                TempData["MsgSucesso"] = "Excluído com sucesso!";
            }
            catch (Exception ex) { TempData["MsgErro"] = ex.Message; }
            return RedirectToAction("Selecionar");
        }

       

        // --- AUXILIARES ---
        private List<SelectListItem> ObterReceitas()
        {
            DataTable dt = new Receitas().SelecionarTodos();
            return (from DataRow dr in dt.Rows
                    select new SelectListItem
                    {
                        Value = dr["idReceita"].ToString(),
                        Text = dr["nomePrato"].ToString()
                    }).ToList();
        }

        private List<SelectListItem> ObterInsumos()
        {
            DataTable dt = new Insumos().SelecionarTodos();
            return (from DataRow dr in dt.Rows
                    select new SelectListItem
                    {
                        Value = dr["idInsumo"].ToString(),
                        
                        Text = $"{dr["nomeInsumo"]} ({dr["unidadeMed"]})"
                    }).ToList();
        }
    }
}