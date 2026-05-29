using System.ComponentModel.DataAnnotations;

namespace K_Gest.Models
{
    public class CadastroViewModel
    {
        [Required(ErrorMessage = "O nome de exibição é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo {1} caracteres.")]
        public string? NomeExibicao { get; set; }

        [Required(ErrorMessage = "O usuário é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome de usuário deve ter no máximo {1} caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9_.]+$", ErrorMessage = "O usuário não pode conter espaços ou caracteres especiais (use apenas letras, números, '.' ou '_').")]
        public string? Usuario { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre {2} e {1} caracteres.")]
        [DataType(DataType.Password)]
        public string? Senha { get; set; }

        [Required(ErrorMessage = "A confirmação de senha é obrigatória.")]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas digitadas não coincidem.")]
        public string? ConfirmaSenha { get; set; }
    }
}