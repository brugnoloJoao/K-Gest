using K_Gest.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

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
        public DataTable ObterListaCompras()
        {
            DataTable dt = new DataTable();

            string sql = @"
            WITH DemandaPrevista AS (
            -- 1. Calcula o consumo estimado puro (em G, ML ou UN) com base nas vendas dos últimos 7 dias
                SELECT 
                    cr.idInsumo,
                    SUM(CAST(hv.qtdVendida AS DECIMAL(10,3)) * cr.qtdNecessaria) AS qtdDemandada
                FROM Historico_Vendas hv
                INNER JOIN Receitas r ON hv.idReceita = r.idReceita
                INNER JOIN Composicao_Receita cr ON r.idReceita = cr.idReceita
                WHERE hv.dataVend >= DATEADD(day, -7, CAST(GETDATE() AS DATE))
                GROUP BY cr.idInsumo
            ),
            AnaliseValidades AS (
             -- 2. Calcula perdas e alertas de lote usando as mesmas unidades da tabela Insumos
                SELECT 
                    idInsumo,
                    SUM(CASE WHEN dtValidade < CAST(GETDATE() AS DATE) THEN CAST(quantidade AS DECIMAL(10,3)) ELSE 0 END) AS qtdVencida,
                    SUM(CASE WHEN dtValidade >= CAST(GETDATE() AS DATE) AND dtValidade <= DATEADD(day, 7, CAST(GETDATE() AS DATE)) THEN CAST(quantidade AS DECIMAL(10,3)) ELSE 0 END) AS qtdVencendoSemana
                FROM Lote
                GROUP BY idInsumo
        )
        -- 3. Consolida os dados brutos (Tudo operando na menor unidade: G, ML, UN)
        SELECT 
            i.idInsumo,
            i.nomeInsumo,
            i.unidadeMed, -- Aqui virá KG ou L, mas sabemos que o valor numérico abaixo está em G ou ML
            (i.estoqueAtual - ISNULL(v.qtdVencida, 0)) AS estoqueAtual,
            i.pontoPedido,
            ISNULL(d.qtdDemandada, 0) AS demandaSemanal,
            ISNULL(v.qtdVencendoSemana, 0) AS qtdVencendoSemana,
            
            -- Fórmula Híbrida Direta: (Vendas + PontoPedido + Vencendo) - Estoque Válido
            ((ISNULL(d.qtdDemandada, 0) + i.pontoPedido + ISNULL(v.qtdVencendoSemana, 0)) - (i.estoqueAtual - ISNULL(v.qtdVencida, 0))) AS qtdSugerida
        FROM Insumos i
        LEFT JOIN DemandaPrevista d ON i.idInsumo = d.idInsumo
        LEFT JOIN AnaliseValidades v ON i.idInsumo = v.idInsumo
        WHERE ((ISNULL(d.qtdDemandada, 0) + i.pontoPedido + ISNULL(v.qtdVencendoSemana, 0)) - (i.estoqueAtual - ISNULL(v.qtdVencida, 0))) > 0
        ORDER BY i.nomeInsumo ASC;";

            try
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
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

            return dt;
        }
    }
}