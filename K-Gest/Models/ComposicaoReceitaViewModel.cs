
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; 

public class ComposicaoReceitaViewModel
{
    [Key]
    public int? idComposicao { get; set; }

    [Required(ErrorMessage = "Digite a quantidade necessária.")]
    [Range(0.01, 999999.99, ErrorMessage = "A quantidade deve ser maior que zero.")]
    [Display(Name = "Quantidade Necessária")]
    public decimal qtdNecessaria { get; set; }

    [Required(ErrorMessage = "Selecione uma receita.")]
    [Display(Name = "Receita")]
    public int idReceita { get; set; }

    [Required(ErrorMessage = "Selecione um insumo.")]
    [Display(Name = "Insumo")]
    public int idInsumo { get; set; }

  
    // Listas para preencher os Selects (Dropdowns) na View
    public List<SelectListItem>? ListaReceitas { get; set; }
    public List<SelectListItem>? ListaInsumos { get; set; }
}