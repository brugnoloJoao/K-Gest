using Microsoft.Data.SqlClient;
using System.Data;

namespace K_Gest.BancoDados
{
    public class Exemplo
    {

        //-------------------------------------------------------------
        // Atributos
        //-------------------------------------------------------------
        public int? idSetor;
        public string nome;
        public string? descricao;


        SqlConnection con;

        //-------------------------------------------------------------
        // Construtor
        //-------------------------------------------------------------
        public Exemplo()
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
        // Métodos
        //-------------------------------------------------------------
        public void Inserir()
        {
            try
            {
                string cmdSQL = "INSERT INTO Setores(Nome, Descricao) VALUES(@Nome, @Descricao)";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@Nome", nome);
                cmd.Parameters.AddWithValue("@Descricao", descricao ?? Convert.DBNull);

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
                string cmdSQL = "UPDATE Setores SET Nome = @Nome, Descricao = @Descricao Where IdSetor = @IdSetor";

                //Prepara SqlCommand
                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@IdSetor", idSetor);
                cmd.Parameters.AddWithValue("@Nome", nome);
                cmd.Parameters.AddWithValue("@Descricao", descricao ?? Convert.DBNull);

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
                string cmdSQL = "Delete From Setores Where IdSetor = @IdSetor";

                //Prepara SqlCommand
                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@IdSetor", idSetor);

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
                string cmdSQL = "SELECT IdSetor, Nome, Descricao FROM Setores ORDER BY IdSetor";

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
                string cmdSQL = "SELECT IdSetor, Nome, Descricao FROM Setores WHERE IdSetor = @IdSetor";

                // Prepara SQL Adapter
                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);

                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@IdSetor", idSetor);

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
