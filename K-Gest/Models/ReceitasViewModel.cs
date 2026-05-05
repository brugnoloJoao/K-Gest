using System.ComponentModel.DataAnnotations;

namespace K_Gest.Models
{
    public class ReceitasViewModel
    {
        public int? IdSetor { get; set; }

        [Required(ErrorMessage = "Digite o nome do Setor.")]
        [MaxLength(100)]
        public string Nome { get; set; }
        public string? Descricao { get; set; }
    }
}
