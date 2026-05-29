using System.ComponentModel.DataAnnotations;
namespace K_Gest.Models
{

    public class ReceitasViewModel
    {
        [Key]
        public int? IdReceita { get; set; }

        [Required(ErrorMessage = "O nome do prato é obrigatório.")]
        [StringLength(150, ErrorMessage = "O nome do prato deve ter no máximo 150 caracteres.")]
        [Display(Name = "Nome do Prato")]
        public string NomePrato { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Display(Name = "Preço")]
        [DataType(DataType.Currency)]
        [Range(0.01, 1000000, ErrorMessage = "O preço deve ser maior que zero.")] 
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal Preco { get; set; }
    }
}