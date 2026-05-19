using Microsoft.AspNetCore.Mvc.Rendering;

namespace K_Gest.Models
{
    public class ComposicaoReceitaViewModel
    {
        // Adicione esta propriedade para resolver os erros CS0117 e CS1061
        public int IdComposicao { get; set; }
        public int IdReceita { get; set; }
        public int IdInsumo { get; set; }
        public decimal QtdNecessaria { get; set; }

        public List<ItemComposicao> Itens { get; set; } = new List<ItemComposicao>();
        public List<SelectListItem>? ListaReceitas { get; set; }
        public List<SelectListItem>? ListaInsumos { get; set; }
    }

    // Esta classe DEVE estar no mesmo arquivo ou no namespace Models
    public class ItemComposicao
    {
        public int IdInsumo { get; set; }
        public string? NomeInsumo { get; set; }
        public decimal Quantidade { get; set; }
        public string? UnidadeMed { get; set; }
    }
}