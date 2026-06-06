using System.ComponentModel.DataAnnotations;

namespace K_Gest.Models
{
    public class InsumosViewModel
    {
        [Key]
        public int? IdInsumo { get; set; }

        [Required(ErrorMessage = "O nome do insumo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome do Insumo")]
        public string NomeInsumo { get; set; }

        [Required(ErrorMessage = "Informe a unidade de medida (Ex: Kg, Un, Litro).")]
        [StringLength(10, ErrorMessage = "A unidade deve ser curta (máx. 10 caracteres).")]
        [Display(Name = "Unidade de Medida")]
        public string UnidadeMed { get; set; }

        [Required(ErrorMessage = "O campo Estoque é obrigatório.")]
        [Range(0, double.MaxValue, ErrorMessage = "O estoque não pode ser negativo.")]
        public decimal? EstoqueAtual { get; set; } 

        [Required(ErrorMessage = "O campo Ponto de Pedido (Estoque mínimo) é obrigatório.")]
        [Range(0, double.MaxValue, ErrorMessage = "O Ponto de Pedido não pode ser negativo.")]
        public decimal? PontoPedido { get; set; } 
    }
}