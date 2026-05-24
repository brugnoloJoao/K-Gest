using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using K_Gest.BancoDados;
using K_Gest.Models;
using System.Data;

namespace K_Gest.Controllers
{
    public class LoteController : Controller
    {// 1. Tela Principal: Mostra os Cards de Insumos
        public IActionResult Selecionar()
        {
            try
            {
                // Busca todos os insumos cadastrados no sistema
                DataTable dtInsumos = new Insumos().SelecionarTodos();
                if (dtInsumos == null) dtInsumos = new DataTable();

                return View("SelecionarView", dtInsumos);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = "Erro ao carregar insumos: " + ex.Message;
                return View("SelecionarView", new DataTable());
            }
        }


        // Deixamos a rota explícita e aceitando parâmetro nomeado também
        [HttpGet]
        public IActionResult GerenciarLotes(int? id)
        {
            try
            {
                // Se o ID veio nulo ou zerado, tenta pegar da QueryString ou redireciona
                int insumoId = id ?? Convert.ToInt32(Request.Query["id"]);

                if (insumoId <= 0)
                {
                    TempData["MsgErro"] = "Código de insumo inválido ou não informado.";
                    return RedirectToAction("Selecionar");
                }

                // Instancia a classe de Insumos para buscar os dados do topo do card
                DataTable dtInsumos = new Insumos().SelecionarTodos();
                DataRow insumoRow = null;

                if (dtInsumos != null)
                {
                    // Busca a linha correspondente testando variações de maiúsculas/minúsculas na coluna de ID
                    insumoRow = dtInsumos.AsEnumerable().FirstOrDefault(r =>
                        (r.Table.Columns.Contains("idInsumo") && Convert.ToInt32(r["idInsumo"]) == insumoId) ||
                        (r.Table.Columns.Contains("IdInsumo") && Convert.ToInt32(r["IdInsumo"]) == insumoId)
                    );
                }

                if (insumoRow == null)
                {
                    TempData["MsgErro"] = "Insumo não encontrado no sistema.";
                    return RedirectToAction("Selecionar");
                }

                // Alimenta as ViewBags de forma segura para a tela de gerenciamento de lotes
                ViewBag.IdInsumo = insumoId;
                ViewBag.NomeInsumo = dtInsumos.Columns.Contains("nomeInsumo") ? insumoRow["nomeInsumo"].ToString() : insumoRow[1].ToString();
                string unidadeMed = dtInsumos.Columns.Contains("unidadeMed") ? insumoRow["unidadeMed"].ToString() : "";
                ViewBag.UnidadeMed = unidadeMed;

                // Busca os lotes vinculados
                Lote o_Lote = new Lote();
                DataTable dtLotes = o_Lote.SelecionarPorInsumo(insumoId);

                if (dtLotes == null)
                {
                    dtLotes = new DataTable();
                }

                foreach (DataRow row in dtLotes.Rows)
                {
                    row["quantidade"] = o_Lote.ConverterParaTela(Convert.ToDecimal(row["quantidade"]), unidadeMed);
                }

                return View("GerenciarLotesView", dtLotes);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = "Erro interno ao carregar os lotes: " + ex.Message;
                return RedirectToAction("Selecionar");
            }
        }

        // 3. Ajuste no InserirExibir para já receber o ID do Insumo selecionado automaticamente
        public IActionResult InserirExibir(int idInsumo)
        {
            var vm = new LoteViewModel
            {
                IdInsumo = idInsumo, // Já deixa o insumo correto pré-selecionado
                ListaInsumos = ObterInsumos()
            };
            return View("InserirExibirView", vm);
        }

        [HttpPost]
        public IActionResult InserirProcessar(LoteViewModel vm)
        {
            // Limpa validações automáticas de listas auxiliares que travam o ModelState.IsValid
            ModelState.Remove("ListaInsumos");
            ModelState.Remove("idLote");

            // Verificação manual simplificada para garantir que os dados obrigatórios chegaram
            if (vm.IdInsumo > 0 && vm.NumLote > 0)
            {
                try
                {
                    // Instancia a classe de persistência configurando os atributos exatamente como seu diagrama
                    Lote o_Lote = new Lote
                    {
                        dtFabricacao = vm.DtFabricacao,
                        dtValidade = vm.DtValidade,
                        numLote = vm.NumLote,
                        idInsumo = vm.IdInsumo,
                        
                    };

                    
                    Insumos o_Insumo = new Insumos();
                    o_Insumo.idInsumo = vm.IdInsumo;
                    
                    
                    decimal estoqueAtual = o_Insumo.ObterEstoqueAtual();
                    string unidadeMed = o_Insumo.ObterUnidadeMedida();

                    o_Lote.quantidade = o_Lote.ConverterParaBanco(vm.Quantidade, unidadeMed);
                    // Executa o comando INSERT no banco de dados
                    o_Lote.Inserir();

                    decimal somaLotes = o_Lote.TotalLotes();

                    if (somaLotes - estoqueAtual > 0)
                    {
                        MovimentacaoEstoque o_MovimentacaoEstoque = new MovimentacaoEstoque
                        {
                            tipoEs = "E",
                            qtdMoviment = somaLotes - estoqueAtual,
                            motivo = "Ajuste automático de estoque após cadastro de lote",
                            idInsumo = vm.IdInsumo
                        };
                        o_MovimentacaoEstoque.InserirPorLote();
                    }

                    TempData["MsgSucesso"] = "Lote cadastrado com sucesso!";

                    // Após salvar, volta para a tela de gerenciamento de lotes daquele insumo específico
                    return RedirectToAction("GerenciarLotes", new { id = vm.IdInsumo });
                }
                catch (Exception ex)
                {
                    // Se der erro no banco, exibe o erro real na barra de mensagens para sabermos o motivo
                    TempData["MsgErro"] = "Erro ao salvar no banco: " + ex.Message;
                }
            }
            else
            {
                TempData["MsgErro"] = "Por favor, preencha todos os campos obrigatórios (Insumo e Número do Lote).";
            }

            // Se falhar ou der erro, recarrega a lista de insumos e mantém o usuário na mesma tela para não perder o que digitou
            vm.ListaInsumos = ObterInsumos();
            return View("InserirExibirView", vm);
        }

        // ==========================================
        // 3. ALTERAÇÃO (Editar Lote)
        // ==========================================
        public IActionResult AlterarExibir(int id)
        {
            try
            {
                Lote o_Lote = new Lote { idLote = id };
                DataTable dt = o_Lote.SelecionarPorID();

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    var vm = new LoteViewModel
                    {
                        IdLote = Convert.ToInt32(row["IdLote"]),
                        DtFabricacao = Convert.ToDateTime(row["DtFabricacao"]),
                        DtValidade = Convert.ToDateTime(row["DtValidade"]),
                        NumLote = Convert.ToInt32(row["NumLote"]),
                        IdInsumo = Convert.ToInt32(row["IdInsumo"]),
                        ListaInsumos = ObterInsumos()
                    };
                    return View("AlterarExibirView", vm);
                }
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = ex.Message;
            }

            return RedirectToAction("Selecionar");
        }

        [HttpPost]
        public IActionResult AlterarProcessar(LoteViewModel vm)
        {
            ModelState.Remove("ListaInsumos");

            try
            {
                Lote o_Lote = new Lote
                {
                    idLote = vm.IdLote,
                    dtFabricacao = vm.DtFabricacao,
                    dtValidade = vm.DtValidade,
                    numLote = vm.NumLote,
                    idInsumo = vm.IdInsumo
                };

                o_Lote.Alterar();

                TempData["MsgSucesso"] = "Lote atualizado com sucesso!";
                return RedirectToAction("Selecionar");
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = ex.Message;
                vm.ListaInsumos = ObterInsumos();
                return View("AlterarExibirView", vm);
            }
        }

        // ==========================================
        // 4. EXCLUSÃO
        // ==========================================
        public IActionResult Excluir(int id)
        {
            
            try
            {
                Lote o_Lote = new Lote { idLote = id };
                int idInsumo = o_Lote.ObterIDInsumoPorIDLote();
                o_Lote.Excluir();
                TempData["MsgSucesso"] = "Lote excluído com sucesso!";
                return RedirectToAction("GerenciarLotes", idInsumo);

            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = "Erro ao excluir: " + ex.Message;
            }
            return RedirectToAction("Selecionar");
        }

        // ==========================================
        // 5. MÉTODOS AUXILIARES
        // ==========================================
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