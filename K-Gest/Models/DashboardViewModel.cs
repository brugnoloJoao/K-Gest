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

        public int LotesEmAlerta { get; set; } // Lotes que vencem nos próximos 15 dias
        public int LotesVencidos { get; set; }

        public decimal TotalVendidoDezDias { get; set; }
        public int QtdPratosVendidos { get; set; }

        // --- 2. Dados do Gráfico de Linha (Fluxo de Estoque) ---
        // Ex: ["20/05", "21/05", "22/05"]
        public List<string> DatasGraficoLinha { get; set; } = new List<string>();
        // Contém a soma de KG/Unidades que entraram em cada um dos dias acima
        public List<decimal> DadosEntradasGrafico { get; set; } = new List<decimal>();
        // Contém a soma de KG/Unidades que saíram em cada um dos dias acima
        public List<decimal> DadosSaidasGrafico { get; set; } = new List<decimal>();

        // --- 3. Dados do Gráfico de Rosquinha (Top Pratos) ---
        // Ex: ["Bolo de Chocolate", "Torta de Frango"]
        public List<string> NomesTopReceitas { get; set; } = new List<string>();
        // Ex: [45, 22] (Quantidades vendidas)
        public List<int> QtdVendasTopReceitas { get; set; } = new List<int>();

        // --- 4. Dados para a Tabela de Atividades Recentes ---
        public List<MovimentacaoItemDashboard> UltimasMovimentacoes { get; set; } = new List<MovimentacaoItemDashboard>();
    }

    /// <summary>
    /// Classe auxiliar para estruturar as linhas da tabela de últimas movimentações.
    /// Ela une dados da tabela Movimentacao_Estoque com dados da tabela Insumos (Nome e Unidade).
    /// </summary>
    public class MovimentacaoItemDashboard
    {
        public string tipoEs { get; set; } = string.Empty; // 'E' para Entrada, 'S' para Saída
        public string NomeInsumo { get; set; } = string.Empty; // Nome do Insumo associado
        public string UnidadeMed { get; set; } = string.Empty; // Unidade (KG, UN, L, etc.)
        public decimal qtdMoviment { get; set; } // Quantidade movimentada
        public DateTime dataMoviment { get; set; } // Data e hora do registro
        public string motivo { get; set; } = string.Empty; // Justificativa da movimentação
    }
}