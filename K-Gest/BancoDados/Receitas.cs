using Microsoft.Data.SqlClient;
using System.Data;

namespace K_Gest.BancoDados
{
    public class Receitas
    {

        //-------------------------------------------------------------
        // Atributos
        //-------------------------------------------------------------
        public int? idReceita;
        public string nomePrato;

        SqlConnection con;

        //-------------------------------------------------------------
        // Construtor
        //-------------------------------------------------------------
        public Receitas()
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
                string cmdSQL = "INSERT INTO Receitas(nomePrato) VALUES(@NomePrato)";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@NomePrato", nomePrato);

                //Abre conexão com BD
                con.Open();

                // Executar o comando SQL
                cmd.ExecuteNonQuery();

                //Executar o comando SQL

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
                // Prepara o comando SQL
                string cmdSQL = "UPDATE Receitas SET nomePrato = @NomePrato WHERE idReceita = @IdReceita";

                //Prepara SqlCommand
                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@IdReceita", idReceita);
                cmd.Parameters.AddWithValue("@NomePrato", nomePrato);

                //Abre conexão com BD
                con.Open();

                // Executar o comando SQL
                cmd.ExecuteNonQuery();

                // Fecha a conexão com o BD
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
                string cmdSQL = "Delete From Setores Where IdSetor = @IdReceita";

                //Prepara SqlCommand
                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@IdReceita", idReceita);

                //Abre conexão com BD
                con.Open();

                // Executar o comando SQL
                cmd.ExecuteNonQuery();

                //Fecha a conexão com o BD
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
                // Prepara o comando SQL
                string cmdSQL = "SELECT idReceita, nomePrato FROM receitas ORDER BY idReceita";

                // Prepara SQL Adapter
                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);

                //Abre conexão com BD
                con.Open();

                DataTable dtPesquisa = new DataTable();

                // Executa o Select no banco de dados
                int qtdeLinhasAfetada = o_DataAdapter.Fill(dtPesquisa);

                // Fecha conexão com BD
                con.Close();

                if (qtdeLinhasAfetada > 0)
                {
                    return dtPesquisa;
                }
                else
                {
                    return null;
                }
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
                // Prepara o comando SQL
                string cmdSQL = "SELECT idReceita, nomePrato FROM receitas WHERE idReceita = @IdReceita";

                // Prepara SQL Adapter
                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);

                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@IdReceita", idReceita);

                //Abre conexão com BD
                con.Open();

                DataTable dtPesquisa = new DataTable();

                // Executa o Select no banco de dados
                int qtdeLinhasAfetada = o_DataAdapter.Fill(dtPesquisa);

                // Fecha conexão com BD
                con.Close();

                if (qtdeLinhasAfetada > 0)
                {
                    return dtPesquisa;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
