using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
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
            // Garante que as listas internas do ViewModel iniciem vazias e nunca nulas
            var model = new DashboardViewModel
            {
                MotivosEntrada = new List<string>(),
                QtdsEntrada = new List<decimal>(),
                MotivosSaida = new List<string>(),
                QtdsSaida = new List<decimal>(),
                UltimasMovimentacoes = new List<MovimentacaoItemDashboard>()
            };

            try
            {
                con.Open();

                // 1. Contar Total de Insumos, Receitas e Alertas
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
                        model.TotalInsumos = dr["TotalInsumos"] != DBNull.Value ? Convert.ToInt32(dr["TotalInsumos"]) : 0;
                        model.TotalReceitas = dr["TotalReceitas"] != DBNull.Value ? Convert.ToInt32(dr["TotalReceitas"]) : 0;
                        model.InsumosAbaixoPontoPedido = dr["AbaixoPonto"] != DBNull.Value ? Convert.ToInt32(dr["AbaixoPonto"]) : 0;
                        model.InsumosZerados = dr["Zerados"] != DBNull.Value ? Convert.ToInt32(dr["Zerados"]) : 0;
                        model.LotesEmAlerta = dr["LotesAlerta"] != DBNull.Value ? Convert.ToInt32(dr["LotesAlerta"]) : 0;
                        model.LotesVencidos = dr["LotesVencidos"] != DBNull.Value ? Convert.ToInt32(dr["LotesVencidos"]) : 0;
                    }
                }

                // 2. Histórico de Vendas (Últimos 10 dias)
                string sqlVendas = @"
                    SELECT ISNULL(SUM(qtdVendida), 0) as Qtd 
                    FROM Historico_Vendas 
                    WHERE dataVend >= DATEADD(day, -10, CAST(GETDATE() AS DATE))";

                using (SqlCommand cmd = new SqlCommand(sqlVendas, con))
                {
                    object resultadoVendas = cmd.ExecuteScalar();
                    model.QtdPratosVendidos = resultadoVendas != DBNull.Value ? Convert.ToInt32(resultadoVendas) : 0;
                    model.TotalVendidoDezDias = model.QtdPratosVendidos * 25.00m;
                }

                // 3. Gráfico de Barras - Entradas por Motivo
                string sqlMotivosEntrada = @"
                    SELECT COALESCE(motivo, 'Não informado') as motivo, COUNT(*) as total 
                    FROM Movimentacao_Estoque 
                    WHERE tipoEs = 'E' OR tipoEs = 'Entrada'
                    GROUP BY motivo";

                using (SqlCommand cmd = new SqlCommand(sqlMotivosEntrada, con))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string motivo = dr["motivo"] != DBNull.Value ? dr["motivo"].ToString()! : "Não informado";
                        decimal total = dr["total"] != DBNull.Value ? Convert.ToDecimal(dr["total"]) : 0m;

                        model.MotivosEntrada.Add(motivo);
                        model.QtdsEntrada.Add(total);
                    }
                }

                // 4. Gráfico de Barras - Saídas por Motivo
                string sqlMotivosSaida = @"
                    SELECT COALESCE(motivo, 'Não informado') as motivo, COUNT(*) as total 
                    FROM Movimentacao_Estoque 
                    WHERE tipoEs = 'S' OR tipoEs = 'Saída'
                    GROUP BY motivo";

                using (SqlCommand cmd = new SqlCommand(sqlMotivosSaida, con))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string motivo = dr["motivo"] != DBNull.Value ? dr["motivo"].ToString()! : "Não informado";
                        decimal total = dr["total"] != DBNull.Value ? Convert.ToDecimal(dr["total"]) : 0m;

                        model.MotivosSaida.Add(motivo);
                        model.QtdsSaida.Add(total);
                    }
                }

                // 5. Tabela: Últimas Movimentações 
                string sqlUltimasMov = @"
                    SELECT TOP 5 m.tipoEs, i.nomeInsumo, i.unidadeMed, m.qtdMoviment, m.dataMoviment, m.motivo
                    FROM Movimentacao_Estoque m
                    INNER JOIN Insumos i ON m.idInsumo = i.idInsumo
                    ORDER BY m.idEstoque DESC";

                using (SqlCommand cmd = new SqlCommand(sqlUltimasMov, con))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        // Tratamento individual seguro para cada campo da tabela
                        string tipoEsTratado = dr["tipoEs"] != DBNull.Value ? dr["tipoEs"].ToString()! : "E";
                        string nomeInsumoTratado = dr["nomeInsumo"] != DBNull.Value ? dr["nomeInsumo"].ToString()! : "Insumo Oculto";
                        string unidadeMedTratada = dr["unidadeMed"] != DBNull.Value ? dr["unidadeMed"].ToString()! : "un";
                        decimal qtdTratada = dr["qtdMoviment"] != DBNull.Value ? Convert.ToDecimal(dr["qtdMoviment"]) : 0m;
                        string motivoTratado = dr["motivo"] != DBNull.Value ? dr["motivo"].ToString()! : "Sem justificativa";

                        DateTime dataTratada = dr["dataMoviment"] == DBNull.Value
                            ? DateTime.Now
                            : Convert.ToDateTime(dr["dataMoviment"]);

                        model.UltimasMovimentacoes.Add(new MovimentacaoItemDashboard
                        {
                            tipoEs = tipoEsTratado,
                            NomeInsumo = nomeInsumoTratado,
                            UnidadeMed = unidadeMedTratada,
                            qtdMoviment = (unidadeMedTratada?.ToUpper() == "KG" || unidadeMedTratada?.ToUpper() == "L") ? qtdTratada / 1000 : qtdTratada,
                            dataMoviment = dataTratada,
                            motivo = motivoTratado
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                con.Close();
            }

            return model;
        }
    }
}