using System.ComponentModel.DataAnnotations;

namespace K_Gest.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O usuário é obrigatório.")]
        public string? Usuario { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string? Senha { get; set; }

        public string? NomeExibicao { get; set; }
        public string? Perfil { get; set; }
    }
}