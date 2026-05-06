using Microsoft.Data.SqlClient;
using System.Data;

namespace K_Gest.BancoDados
{
    public class ComposicaoReceita
    {
        //-------------------------------------------------------------
        // Atributos
        //-------------------------------------------------------------
        public int? idComposicao;
        public decimal qtdNecessaria;
        public int idReceita;
        public int idInsumo;

        SqlConnection con;

        //-------------------------------------------------------------
        // Construtor
        //-------------------------------------------------------------
        public ComposicaoReceita()
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
        // Métodos
        //-------------------------------------------------------------
        public void Inserir()
        {
            try
            {
                string cmdSQL = "INSERT INTO ComposicaoReceita(QtdNecessaria, IdReceita, IdInsumo) VALUES(@QtdNecessaria, @IdReceita, @IdInsumo)";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@QtdNecessaria", qtdNecessaria);
                cmd.Parameters.AddWithValue("@IdReceita", idReceita);
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

        public void Alterar()
        {
            try
            {
                string cmdSQL = "UPDATE ComposicaoReceita SET QtdNecessaria = @QtdNecessaria, IdReceita = @IdReceita, IdInsumo = @IdInsumo WHERE IdComposicao = @IdComposicao";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@IdComposicao", idComposicao);
                cmd.Parameters.AddWithValue("@QtdNecessaria", qtdNecessaria);
                cmd.Parameters.AddWithValue("@IdReceita", idReceita);
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
                string cmdSQL = "DELETE FROM ComposicaoReceita WHERE IdComposicao = @IdComposicao";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@IdComposicao", idComposicao);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataTable SelecionarTodos()
        {
            try
            {
                // SQL com JOIN para buscar nomes em vez de apenas IDs
                string cmdSQL = @"SELECT 
                            C.IdComposicao, 
                            C.QtdNecessaria, 
                            R.Nome AS NomeReceita, 
                            I.Nome AS NomeInsumo 
                          FROM ComposicaoReceita C
                          INNER JOIN Receitas R ON C.IdReceita = R.IdReceita
                          INNER JOIN Insumos I ON C.IdInsumo = I.IdInsumo
                          ORDER BY R.Nome";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                con.Open();
                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                return dtPesquisa.Rows.Count > 0 ? dtPesquisa : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao selecionar composições: " + ex.Message);
            }
        }

        public DataTable SelecionarPorID()
        {
            try
            {
                string cmdSQL = @"SELECT 
                            C.IdComposicao, 
                            C.QtdNecessaria, 
                            C.IdReceita, 
                            C.IdInsumo,
                            R.Nome AS NomeReceita, 
                            I.Nome AS NomeInsumo 
                          FROM ComposicaoReceita C
                          INNER JOIN Receitas R ON C.IdReceita = R.IdReceita
                          INNER JOIN Insumos I ON C.IdInsumo = I.IdInsumo
                          WHERE C.IdComposicao = @IdComposicao";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@IdComposicao", idComposicao);

                con.Open();
                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                return dtPesquisa.Rows.Count > 0 ? dtPesquisa : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar composição por ID: " + ex.Message);
            }
        }
    }
}