using Microsoft.AspNetCore.Mvc.Rendering;

namespace K_Gest.Models
{
    public class ComposicaoReceitaViewModel
    {
        public int IdComposicao { get; set; }
        public int IdReceita { get; set; }
        public int IdInsumo { get; set; }
        public decimal QtdNecessaria { get; set; }

        public List<ItemComposicao> Itens { get; set; } = new List<ItemComposicao>();
        public List<SelectListItem>? ListaReceitas { get; set; }
        public List<SelectListItem>? ListaInsumos { get; set; }
    }

    public class ItemComposicao
    {
        public int IdInsumo { get; set; }
        public string? NomeInsumo { get; set; }
        public decimal Quantidade { get; set; }
        public string? UnidadeExibicao { get; set; }

        // Divide por 1000 apenas se for KG ou L para o utilizador ver "1" em vez de "1000"
        public decimal QuantidadeExibicao => (UnidadeExibicao?.ToUpper() == "KG" || UnidadeExibicao?.ToUpper() == "L") ? Quantidade / 1000 : Quantidade;
    }
}