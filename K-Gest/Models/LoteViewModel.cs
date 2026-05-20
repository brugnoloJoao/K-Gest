// Models/LoteViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K_Gest.Models
{
    public class LoteViewModel
    {
        public int? IdLote { get; set; }
        public DateTime DtFabricacao { get; set; } = DateTime.Now;
        public DateTime DtValidade { get; set; } = DateTime.Now.AddMonths(1);
        public int NumLote { get; set; }
        public int IdInsumo { get; set; }

        public List<SelectListItem>? ListaInsumos { get; set; }
    }
}