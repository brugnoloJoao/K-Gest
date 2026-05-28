using System;
using System.Collections.Generic;

namespace K_Gest.Models
{
    public class DashboardViewModel
    {
        // --- 1. Dados dos Cartões (KPIs Indicadores) ---
        public int TotalInsumos { get; set; }
        public int TotalReceitas { get; set; }
        public int InsumosAbaixoPontoPedido { get; set; }
        public int InsumosZerados { get; set; }
        public int LotesEmAlerta { get; set; }
        public int LotesVencidos { get; set; }
        public decimal TotalVendidoDezDias { get; set; }
        public int QtdPratosVendidos { get; set; }

        // --- 2. Gráficos de Entradas e Saídas por Motivo ---
        public List<string> MotivosEntrada { get; set; } = new List<string>();
        public List<decimal> QtdsEntrada { get; set; } = new List<decimal>();
        public List<string> MotivosSaida { get; set; } = new List<string>();
        public List<decimal> QtdsSaida { get; set; } = new List<decimal>();

        // --- 3. Dados para a Tabela de Atividades Recentes ---
        public List<MovimentacaoItemDashboard> UltimasMovimentacoes { get; set; } = new List<MovimentacaoItemDashboard>();

        // --- 4. Propriedades de Suporte Legadas/Futuras ---
        public List<string> DatasGraficoLinha { get; set; } = new List<string>();
        public List<decimal> DadosEntradasGrafico { get; set; } = new List<decimal>();
        public List<decimal> DadosSaidasGrafico { get; set; } = new List<decimal>();
        public List<string> NomesTopReceitas { get; set; } = new List<string>();
        public List<int> QtdVendasTopReceitas { get; set; } = new List<int>();
    }

    public class MovimentacaoItemDashboard
    {
        public string tipoEs { get; set; } = string.Empty;
        public string NomeInsumo { get; set; } = string.Empty;
        public string UnidadeMed { get; set; } = string.Empty;
        public decimal qtdMoviment { get; set; }
        public DateTime dataMoviment { get; set; }
        public string motivo { get; set; } = string.Empty;
    }
}