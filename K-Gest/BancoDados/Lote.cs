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
                    .AddJsonFile(@".\Configuration\ProjTCC.json")
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
                // Note que o idInsumo é inserido normalmente como um inteiro
                string cmdSQL = "INSERT INTO Lotes(DtFabricacao, DtValidade, NumLote, IdInsumo) " +
                                "VALUES(@DtFabricacao, @DtValidade, @NumLote, @IdInsumo)";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);
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
                throw new Exception("Erro ao inserir lote (Verifique se o Insumo existe): " + ex.Message);
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
        public DataTable SelecionarTodos()
        {
            try
            {
                // Exemplo de SELECT com JOIN para trazer o nome do Insumo junto
                string cmdSQL = @"SELECT L.IdLote, L.DtFabricacao, L.DtValidade, L.NumLote, L.IdInsumo, I.Nome as NomeInsumo 
                                  FROM Lotes L 
                                  INNER JOIN Insumos I ON L.IdInsumo = I.IdInsumo 
                                  ORDER BY L.IdLote";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                con.Open();
                DataTable dtPesquisa = new DataTable();
                int qtdeLinhasAfetada = o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                return qtdeLinhasAfetada > 0 ? dtPesquisa : null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataTable SelecionarPorID()
        {
            try
            {
                string cmdSQL = "SELECT IdLote, DtFabricacao, DtValidade, NumLote, IdInsumo FROM Lotes WHERE IdLote = @IdLote";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@IdLote", idLote);

                con.Open();
                DataTable dtPesquisa = new DataTable();
                int qtdeLinhasAfetada = o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                return qtdeLinhasAfetada > 0 ? dtPesquisa : null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}