using Microsoft.AspNetCore.Mvc.Rendering;
namespace K_Gest.Models
{
    public class ComposicaoReceitaViewModel
    {
        public int IdComposicao { get; set; }
        public decimal QtdNecessaria { get; set; }
        public int IdReceita { get; set; }
        public int IdInsumo { get; set; }

        // Campo informativo vindo da tabela Insumos (FK)
        public string UnidadeMed { get; set; }

        public List<SelectListItem> ListaReceitas { get; set; }
        public List<SelectListItem> ListaInsumos { get; set; }
    }


}
