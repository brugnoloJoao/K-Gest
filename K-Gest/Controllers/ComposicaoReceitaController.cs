using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using K_Gest.BancoDados;
using K_Gest.Models;
using System.Data;

namespace K_Gest.Controllers
{
    public class ComposicaoReceitaController : Controller
    {
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
                decimal qtdOriginal = Convert.ToDecimal(row["qtdNecessaria"]);
                string unidadeEx = row["unidadeExibicao"].ToString() ?? "";
                decimal qtdEx = (unidadeEx.ToUpper() == "KG" || unidadeEx.ToUpper() == "L") ? qtdOriginal / 1000 : qtdOriginal;

                lista.Add(new
                {
                    idComp = row["idComposicao"].ToString(),
                    nomeInsumo = row["nomeInsumo"].ToString(),
                    // Formata removendo zeros desnecessários à direita na string
                    qtdEx = qtdEx.ToString("G29", System.Globalization.CultureInfo.CurrentCulture),
                    unidadeEx = unidadeEx
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
                        IdInsumo = Convert.ToInt32(row["idInsumo"]),
                        NomeInsumo = row["nomeInsumo"].ToString(),
                        Quantidade = Convert.ToDecimal(row["qtdNecessaria"]),
                        UnidadeExibicao = row["unidadeExibicao"].ToString()
                    });
                }
                return View("InserirExibirView", vm);
            }
            catch (Exception ex)
            {
                return Content("Erro ao carregar: " + ex.Message);
            }
        }

        public IActionResult ExcluirIngrediente(int id)
        {
            new ComposicaoReceita().ExcluirIngredienteIndividual(id);
            return RedirectToAction("Selecionar");
        }

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

        [HttpPost]
        public IActionResult AlterarProcessar(ComposicaoReceitaViewModel vm)
        {
            ModelState.Remove("ListaReceitas");
            ModelState.Remove("ListaInsumos");

            try
            {
                if (vm.IdReceita > 0)
                {
                    ComposicaoReceita o_Comp = new ComposicaoReceita();
                    var itensSalvar = vm.Itens ?? new List<ItemComposicao>();

                    o_Comp.AtualizarFichaTecnica(vm.IdReceita, itensSalvar);

                    TempData["MsgSucesso"] = "Ficha Técnica atualizada com sucesso!";
                    return RedirectToAction("Selecionar");
                }
                else
                {
                    TempData["MsgErro"] = "Receita inválida.";
                }
            }
            catch (Exception ex) { TempData["MsgErro"] = ex.Message; }

            vm.ListaReceitas = ObterReceitas();
            vm.ListaInsumos = ObterInsumos();
            return View("InserirExibirView", vm);
        }

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