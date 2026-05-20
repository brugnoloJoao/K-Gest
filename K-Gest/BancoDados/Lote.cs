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
        // Métodos de Persistência
        //-------------------------------------------------------------
        public void Inserir()
        {
            try
            {
                // CORRIGIDO: Nome da tabela para 'Lote' (no singular) e colunas batendo 100% com o diagrama físico
                string cmdSQL = "INSERT INTO Lote(dtFabricacao, dtValidade, numLote, idInsumo) " +
                                "VALUES(@dtFabricacao, @dtValidade, @numLote, @idInsumo)";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                // Vincula os parâmetros mapeando os atributos da classe
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
                // Se a conexão estiver aberta por erro, tenta fechar antes de estourar a exceção
                if (con.State == ConnectionState.Open) con.Close();

                throw new Exception("Erro SQL ao inserir: " + ex.Message);
            }
        }

        public void Alterar()
        {
            try
            {
                string cmdSQL = "UPDATE Lotes SET DtFabricacao = @DtFabricacao, DtValidade = @DtValidade, " +
                                "NumLote = @NumLote, IdInsumo = @IdInsumo WHERE IdLote = @IdLote";

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
                throw new Exception(ex.Message);
            }
        }

        public void Excluir()
        {
            try
            {
                string cmdSQL = "DELETE FROM Lotes WHERE IdLote = @IdLote";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@IdLote", idLote);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //-------------------------------------------------------------
        // Métodos de Consulta (Com JOIN para facilitar a visualização)
        //-------------------------------------------------------------
        //-------------------------------------------------------------
        // Métodos de Consulta Corrigidos com base no Diagrama Real
        //-------------------------------------------------------------

        public DataTable SelecionarTodos()
        {
            try
            {
                // CORRIGIDO: Tabela 'Lote' no singular e colunas batendo com o diagrama
                string cmdSQL = @"SELECT L.idLote, L.dtFabricacao, L.dtValidade, L.numLote, L.idInsumo, I.nomeInsumo as NomeInsumo 
                          FROM Lote L 
                          INNER JOIN Insumos I ON L.idInsumo = I.idInsumo 
                          ORDER BY L.idLote";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                con.Open();
                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                return dtPesquisa.Rows.Count > 0 ? dtPesquisa : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro em SelecionarTodos: " + ex.Message);
            }
        }

        public DataTable SelecionarPorInsumo(int idInsumoBusca)
        {
            try
            {
                // CORRIGIDO: Tabela 'Lote' no singular e mapeamento exato
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

                return dtPesquisa;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro em SelecionarPorInsumo: " + ex.Message);
            }
        }

        public DataTable SelecionarPorID()
        {
            try
            {
                // CORRIGIDO: Tabela 'Lote' no singular e campos em camelCase
                string cmdSQL = "SELECT idLote, dtFabricacao, dtValidade, numLote, idInsumo FROM Lote WHERE idLote = @idLote";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@idLote", idLote);

                con.Open();
                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                return dtPesquisa.Rows.Count > 0 ? dtPesquisa : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro em SelecionarPorID: " + ex.Message);
            }
        }
    }
}