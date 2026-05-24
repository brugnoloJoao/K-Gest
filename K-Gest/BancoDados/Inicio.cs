using Microsoft.Data.SqlClient;
using System.Data;
using K_Gest.Models;

namespace K_Gest.BancoDados
{
    public class Inicio
    {
        private readonly SqlConnection con;

        public Inicio()
        {
            IConfigurationRoot o_Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(@".\Configuration\Kantine.json").Build();
            con = new SqlConnection(o_Config.GetConnectionString("StringConexaoSQLServer"));
        }

        public DashboardViewModel CarregarDadosDashboard()
        {
            var model = new DashboardViewModel();

            try
            {
                con.Open();

                // 1. Contar Total de Insumos e Receitas (Totalmente fiel ao seu diagrama)
                string sqlContadores = @"
                    SELECT 
                        (SELECT COUNT(*) FROM Insumos) as TotalInsumos,
                        (SELECT COUNT(*) FROM Receitas) as TotalReceitas,
                        (SELECT COUNT(*) FROM Insumos WHERE estoqueAtual <= pontoPedido AND estoqueAtual > 0) as AbaixoPonto,
                        (SELECT COUNT(*) FROM Insumos WHERE estoqueAtual <= 0) as Zerados,
                        (SELECT COUNT(*) FROM Lote WHERE dtValidade >= GETDATE() AND dtValidade <= DATEADD(day, 15, GETDATE())) as LotesAlerta,
                        (SELECT COUNT(*) FROM Lote WHERE dtValidade < GETDATE()) as LotesVencidos";

                using (SqlCommand cmd = new SqlCommand(sqlContadores, con))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        model.TotalInsumos = Convert.ToInt32(dr["TotalInsumos"]);
                        model.TotalReceitas = Convert.ToInt32(dr["TotalReceitas"]);
                        model.InsumosAbaixoPontoPedido = Convert.ToInt32(dr["AbaixoPonto"]);
                        model.InsumosZerados = Convert.ToInt32(dr["Zerados"]);
                        model.LotesEmAlerta = Convert.ToInt32(dr["LotesAlerta"]);
                        model.LotesVencidos = Convert.ToInt32(dr["LotesVencidos"]);
                    }
                }

                // 2. Histórico de Vendas (Últimos 10 dias) - Usando 'dataVend' correta
                string sqlVendas = @"
                    SELECT ISNULL(SUM(qtdVendida), 0) as Qtd 
                    FROM Historico_Vendas 
                    WHERE dataVend >= DATEADD(day, -10, CAST(GETDATE() AS DATE))";

                using (SqlCommand cmd = new SqlCommand(sqlVendas, con))
                {
                    model.QtdPratosVendidos = Convert.ToInt32(cmd.ExecuteScalar());
                    model.TotalVendidoDezDias = model.QtdPratosVendidos * 25.00m;
                }

                // 3. Gráfico de Linha Ajustado: Puxa Entradas da fabricação de Lotes e Saídas do Histórico de Vendas
                string sqlGraficoLinha = @"
                    SELECT 
                        Datas.DiaFormatado,
                        ISNULL(Entradas.QtdEntrada, 0) as Entradas,
                        ISNULL(Saidas.QtdSaida, 0) as Saidas
                    FROM (
                        SELECT DISTINCT FORMAT(dtFabricacao, 'dd/MM') as DiaFormatado, CAST(dtFabricacao AS DATE) as DataPura FROM Lote WHERE dtFabricacao >= DATEADD(day, -7, GETDATE())
                        UNION
                        SELECT DISTINCT FORMAT(dataVend, 'dd/MM') as DiaFormatado, CAST(dataVend AS DATE) as DataPura FROM Historico_Vendas WHERE dataVend >= DATEADD(day, -7, GETDATE())
                    ) Datas
                    LEFT JOIN (
                        SELECT FORMAT(dtFabricacao, 'dd/MM') as DiaFormatado, SUM(1) as QtdEntrada 
                        FROM Lote GROUP BY FORMAT(dtFabricacao, 'dd/MM')
                    ) Entradas ON Datas.DiaFormatado = Entradas.DiaFormatado
                    LEFT JOIN (
                        SELECT FORMAT(dataVend, 'dd/MM') as DiaFormatado, SUM(qtdVendida) as QtdSaida 
                        FROM Historico_Vendas GROUP BY FORMAT(dataVend, 'dd/MM')
                    ) Saidas ON Datas.DiaFormatado = Saidas.DiaFormatado
                    ORDER BY Datas.DataPura ASC";

                using (SqlCommand cmd = new SqlCommand(sqlGraficoLinha, con))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        model.DatasGraficoLinha.Add(dr["DiaFormatado"].ToString()!);
                        model.DadosEntradasGrafico.Add(Convert.ToDecimal(dr["Entradas"]));
                        model.DadosSaidasGrafico.Add(Convert.ToDecimal(dr["Saidas"]));
                    }
                }

                if (model.DatasGraficoLinha.Count == 0)
                {
                    model.DatasGraficoLinha.Add(DateTime.Now.ToString("dd/MM"));
                    model.DadosEntradasGrafico.Add(0);
                    model.DadosSaidasGrafico.Add(0);
                }

                // 4. Gráfico de Rosquinha: Top 5 Receitas Mais Vendidas
                string sqlTopReceitas = @"
                    SELECT TOP 5 r.nomePrato, SUM(h.qtdVendida) as Total
                    FROM Historico_Vendas h
                    INNER JOIN Receitas r ON h.idReceita = r.idReceita
                    GROUP BY r.nomePrato
                    ORDER BY Total DESC";

                using (SqlCommand cmd = new SqlCommand(sqlTopReceitas, con))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        model.NomesTopReceitas.Add(dr["nomePrato"].ToString()!);
                        model.QtdVendasTopReceitas.Add(Convert.ToInt32(dr["Total"]));
                    }
                }

                // 5. Tabela: Últimas Movimentações (Adaptada para ler sem a coluna de data)
                string sqlUltimasMov = @"
                    SELECT TOP 5 m.tipoEs, i.nomeInsumo, i.unidadeMed, m.qtdMoviment, m.motivo
                    FROM Movimentacao_Estoque m
                    INNER JOIN Insumos i ON m.idInsumo = i.idInsumo
                    ORDER BY m.idEstoque DESC"; // Ordena pelo ID incremental já que não há data

                using (SqlCommand cmd = new SqlCommand(sqlUltimasMov, con))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        model.UltimasMovimentacoes.Add(new MovimentacaoItemDashboard
                        {
                            tipoEs = dr["tipoEs"].ToString()!,
                            NomeInsumo = dr["nomeInsumo"].ToString()!,
                            UnidadeMed = dr["unidadeMed"].ToString()!,
                            qtdMoviment = Convert.ToDecimal(dr["qtdMoviment"]),
                            dataMoviment = DateTime.Now, // Fallback visual seguro
                            motivo = dr["motivo"].ToString()!
                        });
                    }
                }

                con.Close();
                return model;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro ao processar dados do Dashboard: " + ex.Message);
            }
        }
    }
}