using System.ComponentModel.DataAnnotations;

namespace K_Gest.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O usuário é obrigatório.")]
        [StringLength(50, ErrorMessage = "O usuário não pode exceder 50 caracteres.")]
        public string? Usuario { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string? Senha { get; set; }
    }
}