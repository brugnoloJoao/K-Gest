using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace K_Gest.Models
{
    public class HistoricoVendasViewModel
    {
        public int? IdVendas { get; set; }

        [Required(ErrorMessage = "Data de venda é obrigatória.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Data de Venda")]
        public DateTime DataVend { get; set; }

        [Required(ErrorMessage = "Quantidade vendida é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero.")]
        [Display(Name = "Quantidade Vendida")]
        public int QtdVendida { get; set; }

        // Altere de int para int?
        [Required(ErrorMessage = "Receita é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "IdReceita inválido.")]
        [Display(Name = "Receita")]
        public int? IdReceita { get; set; }
        public List<SelectListItem>? ListaReceitas { get; set; }
    }
}
