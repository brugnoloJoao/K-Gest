using System.ComponentModel.DataAnnotations;

public class InsumoViewModel
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
    [Range(0, 9999999.99, ErrorMessage = "O estoque não pode ser negativo.")]
    [Display(Name = "Estoque Atual")]
    public decimal EstoqueAtual { get; set; }

    [Required(ErrorMessage = "Informe o ponto de pedido.")]
    [Range(0, 9999999.99, ErrorMessage = "O ponto de pedido não pode ser negativo.")]
    [Display(Name = "Ponto de Pedido")]
    public decimal PontoPedido { get; set; }
}