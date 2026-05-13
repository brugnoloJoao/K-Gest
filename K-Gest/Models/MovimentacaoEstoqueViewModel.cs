using System.ComponentModel.DataAnnotations;

namespace K_Gest.Models
{
    public class MovimentacaoEstoqueViewModel
    {
        public int? IdEstoque { get; set; }

        [Required(ErrorMessage = "Tipo de movimentação é obrigatório.")]
        [StringLength(50, ErrorMessage = "O tipo deve ter no máximo 50 caracteres.")]
        [Display(Name = "Tipo de Movimentação")]
        public string TipoEs { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantidade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero.")]
        [Display(Name = "Quantidade")]
        public int QtdMoviment { get; set; }

        [StringLength(500, ErrorMessage = "O motivo deve ter no máximo 500 caracteres.")]
        [Display(Name = "Motivo")]
        public string? Motivo { get; set; }

        [Required(ErrorMessage = "Insumo é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "IdInsumo inválido.")]
        [Display(Name = "Insumo")]
        public int IdInsumo { get; set; }

        // PROPRIEDADE NOVA: Necessária para a conversão de unidades (KG, L, G, ML)
        public string UnidadeMed { get; set; } = string.Empty;
    }
}