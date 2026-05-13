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
        [Required(ErrorMessage = "Informe o estoque atual.")]
        [Display(Name = "Estoque Atual")]
        public decimal EstoqueAtual { get; set; }

        [Required(ErrorMessage = "Informe o ponto de pedido.")]
        [Display(Name = "Ponto de Pedido")]
        public decimal PontoPedido { get; set; }
    }
}