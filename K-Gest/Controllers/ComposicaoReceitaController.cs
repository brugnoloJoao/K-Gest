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
            DataTable dt = o_Comp.SelecionarTodos();
            return View("SelecionarView", dt);
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

            if (vm.IdReceita > 0 && vm.IdInsumo > 0)
            {
                try
                {
                    ComposicaoReceita o_Comp = new ComposicaoReceita
                    {
                        qtdNecessaria = vm.QtdNecessaria,
                        idReceita = vm.IdReceita,
                        idInsumo = vm.IdInsumo
                    };
                    o_Comp.Inserir();
                    TempData["MsgSucesso"] = "Salvo com sucesso!";
                    return RedirectToAction("Selecionar");
                }
                catch (Exception ex) { TempData["MsgErro"] = ex.Message; }
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