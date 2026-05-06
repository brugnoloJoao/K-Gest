using Microsoft.Data.SqlClient;
using System.Data;

namespace K_Gest.BancoDados
{
    public class HistoricoVendas
    {
        //-------------------------------------------------------------
        // Atributos
        //-------------------------------------------------------------
        public int? idVendas;
        public DateTime dataVend;
        public int qtdVendida;
        public int idReceita;

        SqlConnection con;

        //-------------------------------------------------------------
        // Construtor
        //-------------------------------------------------------------
        public HistoricoVendas()
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
                string cmdSQL = "INSERT INTO HistoricoVendas(DataVend, QtdVendida, IdReceita) VALUES(@DataVend, @QtdVendida, @IdReceita)";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@DataVend", dataVend);
                cmd.Parameters.AddWithValue("@QtdVendida", qtdVendida);
                cmd.Parameters.AddWithValue("@IdReceita", idReceita);

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
                string cmdSQL = "UPDATE HistoricoVendas SET DataVend = @DataVend, QtdVendida = @QtdVendida, IdReceita = @IdReceita WHERE IdVendas = @IdVendas";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@IdVendas", idVendas);
                cmd.Parameters.AddWithValue("@DataVend", dataVend);
                cmd.Parameters.AddWithValue("@QtdVendida", qtdVendida);
                cmd.Parameters.AddWithValue("@IdReceita", idReceita);

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
                string cmdSQL = "DELETE FROM HistoricoVendas WHERE IdVendas = @IdVendas";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@IdVendas", idVendas);

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
                // JOIN com a tabela Receitas para trazer o nome do produto vendido
                string cmdSQL = @"SELECT 
                                    H.IdVendas, 
                                    H.DataVend, 
                                    H.QtdVendida, 
                                    R.Nome AS NomeReceita 
                                  FROM HistoricoVendas H
                                  INNER JOIN Receitas R ON H.IdReceita = R.IdReceita
                                  ORDER BY H.DataVend DESC"; // Ordenado pelas vendas mais recentes

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                con.Open();
                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                return dtPesquisa.Rows.Count > 0 ? dtPesquisa : null;
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
                string cmdSQL = @"SELECT 
                                    H.IdVendas, 
                                    H.DataVend, 
                                    H.QtdVendida, 
                                    H.IdReceita,
                                    R.Nome AS NomeReceita 
                                  FROM HistoricoVendas H
                                  INNER JOIN Receitas R ON H.IdReceita = R.IdReceita
                                  WHERE H.IdVendas = @IdVendas";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@IdVendas", idVendas);

                con.Open();
                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                return dtPesquisa.Rows.Count > 0 ? dtPesquisa : null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}