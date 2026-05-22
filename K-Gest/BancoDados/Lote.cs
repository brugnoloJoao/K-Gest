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
            else if (diasRestantes == 1)
            {
                return "Alerta"; // Exatamente 1 dia antes da data de vencimento
            }
            else if (diasRestantes >= 2 && diasRestantes <= 15)
            {
                return "Próximo da data de vencimento"; // De 2 até 15 dias antes
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
                string cmdSQL = "INSERT INTO Lote(dtFabricacao, dtValidade, numLote, idInsumo) " +
                                "VALUES(@dtFabricacao, @dtValidade, @numLote, @idInsumo)";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@dtFabricacao", dtFabricacao);
                cmd.Parameters.AddWithValue("@dtValidade", dtValidade);
                cmd.Parameters.AddWithValue("@numLote", numLote);
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
                                "numLote = @NumLote, idInsumo = @IdInsumo WHERE idLote = @IdLote";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@IdLote", idLote);
                cmd.Parameters.AddWithValue("@DtFabricacao", dtFabricacao);
                cmd.Parameters.AddWithValue("@DtValidade", dtValidade);
                cmd.Parameters.AddWithValue("@NumLote", numLote);
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
                string cmdSQL = @"SELECT idLote, dtFabricacao, dtValidade, numLote, idInsumo 
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
                string cmdSQL = "SELECT idLote, dtFabricacao, dtValidade, numLote, idInsumo FROM Lote WHERE idLote = @idLote";

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
    }
}