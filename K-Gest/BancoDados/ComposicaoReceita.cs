using Microsoft.Data.SqlClient;
using System.Data;

namespace K_Gest.BancoDados
{
    public class ComposicaoReceita
    {
        public int? idComposicao;
        public decimal qtdNecessaria;
        public int idReceita;
        public int idInsumo;

        SqlConnection con;

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
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public void Inserir()
        {
            try
            {
                // Nome corrigido conforme image_dad75e.png
                string cmdSQL = "INSERT INTO Composicao_Receita (qtdNecessaria, idReceita, idInsumo) VALUES(@qtd, @idR, @idI)";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@qtd", qtdNecessaria);
                cmd.Parameters.AddWithValue("@idR", idReceita);
                cmd.Parameters.AddWithValue("@idI", idInsumo);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public void Alterar()
        {
            try
            {
                string cmdSQL = "UPDATE Composicao_Receita SET qtdNecessaria = @qtd, idReceita = @idR, idInsumo = @idI WHERE idComposicao = @idC";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@idC", idComposicao);
                cmd.Parameters.AddWithValue("@qtd", qtdNecessaria);
                cmd.Parameters.AddWithValue("@idR", idReceita);
                cmd.Parameters.AddWithValue("@idI", idInsumo);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public void Excluir()
        {
            try
            {
                string cmdSQL = "DELETE FROM Composicao_Receita WHERE idComposicao = @idC";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@idC", idComposicao);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }



        public DataTable SelecionarTodos()
        {
            try
            {
                string cmdSQL = @"SELECT 
                            C.idComposicao, 
                            C.qtdNecessaria, 
                            R.nomePrato, 
                            I.nomeInsumo,
                            I.unidadeMed
                          FROM Composicao_Receita C
                          INNER JOIN Receitas R ON C.idReceita = R.idReceita
                          INNER JOIN Insumos I ON C.idInsumo = I.idInsumo
                          ORDER BY R.nomePrato";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                DataTable dt = new DataTable();
                o_DataAdapter.Fill(dt);
                return dt;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public DataTable SelecionarPorID()
        {
            try
            {
                string cmdSQL = "SELECT * FROM Composicao_Receita WHERE idComposicao = @idC";
                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@idC", idComposicao);
                DataTable dt = new DataTable();
                o_DataAdapter.Fill(dt);
                return dt;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}