using Microsoft.Data.SqlClient;
using System.Data;

namespace K_Gest.BancoDados
{
    public class Lote
    {
        //-------------------------------------------------------------
        // Atributos
        //-------------------------------------------------------------
        public int? idLote;
        public DateTime dtFabricacao;
        public DateTime dtValidade;
        public int numLote;
        public decimal quantidade;
        public int idInsumo; // Chave Estrangeira

        // Propriedade calculada para obter o status em tempo real do lote
        public string Situacao => CalcularSituacao(dtValidade);

        SqlConnection con;

        //-------------------------------------------------------------
        // Construtor
        //-------------------------------------------------------------
        public Lote()
        {
            try
            {
                IConfigurationRoot o_Config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(@".\Configuration\Kantine.json")
                    .Build();

                string strConexao = o_Config.GetConnectionString(@"StringConexaoSQLServer");
                con = new SqlConnection(strConexao);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //-------------------------------------------------------------
        // Regra de Negócio: Categorias de Vencimento
        //-------------------------------------------------------------
        public static string CalcularSituacao(DateTime dataValidade)
        {
            // Zera as horas para comparar puramente os dias civis
            DateTime hoje = DateTime.Today;
            DateTime validade = dataValidade.Date;

            // Calcula a diferença de dias (Data de Validade menos Hoje)
            int diasRestantes = (validade - hoje).Days;

            if (diasRestantes < 0)
            {
                return "Vencido"; // A partir do primeiro dia de vencimento
            }
            else if (diasRestantes <= 3)
            {
                return "Alerta"; // Exatamente 3 dias antes da data de vencimento
            }
            else if (diasRestantes > 3 && diasRestantes <= 15)
            {
                return "Próximo da data de vencimento"; // De 3 até 15 dias antes
            }
            else
            {
                return "Regular / Próprio para consumo"; // 16 dias ou mais antes da validade
            }
        }

        //-------------------------------------------------------------
        // Métodos de Persistência
        //-------------------------------------------------------------
        public void Inserir()
        {
            try
            {
                string cmdSQL = "INSERT INTO Lote(dtFabricacao, dtValidade, numLote, idInsumo, quantidade) " +
                                "VALUES(@dtFabricacao, @dtValidade, @numLote, @idInsumo, @quantidade)";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@dtFabricacao", dtFabricacao);
                cmd.Parameters.AddWithValue("@dtValidade", dtValidade);
                cmd.Parameters.AddWithValue("@numLote", numLote);
                cmd.Parameters.AddWithValue("@quantidade", quantidade);
                cmd.Parameters.AddWithValue("@idInsumo", idInsumo);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro SQL ao inserir: " + ex.Message);
            }
        }

        public void Alterar()
        {
            try
            {
                // AJUSTADO: Mudado de 'Lotes' para 'Lote' para manter consistência com o banco
                string cmdSQL = "UPDATE Lote SET dtFabricacao = @DtFabricacao, dtValidade = @DtValidade, " +
                                "numLote = @NumLote quantidade = @Quantidade, idInsumo = @IdInsumo WHERE idLote = @IdLote";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@IdLote", idLote);
                cmd.Parameters.AddWithValue("@DtFabricacao", dtFabricacao);
                cmd.Parameters.AddWithValue("@DtValidade", dtValidade);
                cmd.Parameters.AddWithValue("@NumLote", numLote);
                cmd.Parameters.AddWithValue("@Quantidade", quantidade);
                cmd.Parameters.AddWithValue("@IdInsumo", idInsumo);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro SQL ao alterar: " + ex.Message);
            }
        }

        public void Excluir()
        {
            try
            {
                // AJUSTADO: Mudado de 'Lotes' para 'Lote'
                string cmdSQL = "DELETE FROM Lote WHERE idLote = @IdLote";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@IdLote", idLote);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro SQL ao excluir: " + ex.Message);
            }
        }

        //-------------------------------------------------------------
        // Métodos de Consulta
        //-------------------------------------------------------------
        public DataTable SelecionarTodos()
        {
            try
            {
                string cmdSQL = @"SELECT L.idLote, L.dtFabricacao, L.dtValidade, L.numLote, L.idInsumo, I.nomeInsumo as NomeInsumo 
                          FROM Lote L 
                          INNER JOIN Insumos I ON L.idInsumo = I.idInsumo 
                          ORDER BY L.idLote";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                con.Open();
                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                // Adiciona dinamicamente uma coluna de Situação no DataTable retornado para a sua GridView/Tabela ler direto
                if (dtPesquisa.Rows.Count > 0)
                {
                    dtPesquisa.Columns.Add("situacao", typeof(string));
                    foreach (DataRow row in dtPesquisa.Rows)
                    {
                        DateTime validade = Convert.ToDateTime(row["dtValidade"]);
                        row["situacao"] = CalcularSituacao(validade);
                    }
                    return dtPesquisa;
                }

                return null;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro em SelecionarTodos: " + ex.Message);
            }
        }

        public DataTable SelecionarPorInsumo(int idInsumoBusca)
        {
            try
            {
                string cmdSQL = @"SELECT idLote, dtFabricacao, dtValidade, numLote, quantidade, idInsumo
                          FROM Lote 
                          WHERE idInsumo = @idInsumo 
                          ORDER BY dtValidade ASC";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@idInsumo", idInsumoBusca);

                con.Open();
                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                if (dtPesquisa.Rows.Count > 0)
                {
                    dtPesquisa.Columns.Add("situacao", typeof(string));
                    foreach (DataRow row in dtPesquisa.Rows)
                    {
                        DateTime validade = Convert.ToDateTime(row["dtValidade"]);
                        row["situacao"] = CalcularSituacao(validade);
                    }
                }

                return dtPesquisa;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro em SelecionarPorInsumo: " + ex.Message);
            }
        }

        public DataTable SelecionarPorID()
        {
            try
            {
                string cmdSQL = "SELECT idLote, dtFabricacao, dtValidade, numLote, quantidade, idInsumo FROM Lote WHERE idLote = @idLote";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@idLote", idLote);

                con.Open();
                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                if (dtPesquisa.Rows.Count > 0)
                {
                    dtPesquisa.Columns.Add("situacao", typeof(string));
                    foreach (DataRow row in dtPesquisa.Rows)
                    {
                        DateTime validade = Convert.ToDateTime(row["dtValidade"]);
                        row["situacao"] = CalcularSituacao(validade);
                    }
                    return dtPesquisa;
                }

                return null;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro em SelecionarPorID: " + ex.Message);
            }
        }
        public int ObterIDInsumoPorIDLote()
        {
            try
            {
                string cmdSQL = "SELECT idInsumo FROM Lote WHERE idLote = @idLote";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@idLote", idLote);

                con.Open();
                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                if (dtPesquisa.Rows.Count > 0)
                {
                    foreach (DataRow row in dtPesquisa.Rows)
                    {
                        idInsumo = int.Parse(row["idInsumo"].ToString());
                    }
                }

                return idInsumo;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro em SelecionarPorID: " + ex.Message);
            }
        }

        public decimal TotalLotes()
        {
            try
            {
                string cmdSQL = "SELECT SUM(quantidade) as TotalQuantidade FROM Lote WHERE idInsumo = @idInsumo";
                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@idInsumo", idInsumo);
                con.Open();
                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);
                con.Close();
                if (dtPesquisa.Rows.Count > 0 && dtPesquisa.Rows[0]["TotalQuantidade"] != DBNull.Value)
                {
                    return Convert.ToDecimal(dtPesquisa.Rows[0]["TotalQuantidade"]);
                }
                return 0;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro em TotalLotes: " + ex.Message);
            }
        }
        public void AtualizarQuantidade()
        {
            try
            {
                string cmdSQL = "UPDATE Lote SET quantidade = @Quantidade WHERE idLote = @IdLote";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@IdLote", idLote);
                cmd.Parameters.AddWithValue("@Quantidade", quantidade);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro SQL ao atualizar quantidade: " + ex.Message);
            }
        }
        // --- MÉTODOS DE CONVERSÃO ---

        public decimal ConverterParaBanco(decimal valor, string unidade)
        {
            if (string.IsNullOrEmpty(unidade)) return valor;
            switch (unidade.ToUpper())
            {
                case "KG":
                case "L":
                    return valor * 1000; // Converte para gramas ou mililitros
                default:
                    return valor;
            }
        }

        public decimal ConverterParaTela(decimal valor, string unidade)
        {
            if (string.IsNullOrEmpty(unidade)) return valor;
            switch (unidade.ToUpper())
            {
                case "KG":
                case "L":
                    return valor / 1000; // Converte de volta para KG ou L
                default:
                    return valor;
            }
        }

        public int ProcessarLotesVencidos()
        {
            try
            {
                string cmdSQL = "SELECT idLote, idInsumo, quantidade FROM Lote WHERE dtValidade < CAST(GETDATE() AS DATE) AND quantidade > 0;";
                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                con.Open();
                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);
                con.Close();
                if(dtPesquisa.Rows.Count > 0)
                {
                    foreach (DataRow row in dtPesquisa.Rows)
                    {
                        MovimentacaoEstoque o_MovimentacaoEstoque = new MovimentacaoEstoque
                        {
                            tipoEs = "S",
                            qtdMoviment = Convert.ToDecimal(row["quantidade"].ToString()),
                            motivo = "Descarte Automático - Produto Vencido",
                            idInsumo = Convert.ToInt32(row["idInsumo"].ToString()),
                        };
                        o_MovimentacaoEstoque.InserirPorLote(); // Registra a saída do estoque por motivo de vencimento

                        Lote o_Lote = new Lote
                        {
                            idLote = Convert.ToInt32(row["idLote"].ToString()),
                            quantidade = 0, // Zera a quantidade do lote vencido
                        };
                        o_Lote.AtualizarQuantidade(); // Método para atualizar apenas a quantidade do lote
                    }
                    return dtPesquisa.Rows.Count;
                }
                return 0;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro ao processar lotes vencidos: " + ex.Message);
            }
        }
    }
}