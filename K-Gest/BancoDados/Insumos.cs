using Microsoft.Data.SqlClient;
using System.Data;

namespace K_Gest.BancoDados
{
    public class Insumos
    {
        //-------------------------------------------------------------
        // Atributos
        //-------------------------------------------------------------
        public int? idInsumo;
        public string nomeInsumo;
        public string unidadeMed;
        public decimal estoqueAtual;
        public decimal pontoPedido;

        SqlConnection con;

        //-------------------------------------------------------------
        // Construtor
        //-------------------------------------------------------------
        public Insumos()
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
                string cmdSQL = "INSERT INTO Insumos(NomeInsumo, UnidadeMed, EstoqueAtual, PontoPedido) VALUES(@NomeInsumo, @UnidadeMed, @EstoqueAtual, @PontoPedido)";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@NomeInsumo", nomeInsumo);
                cmd.Parameters.AddWithValue("@UnidadeMed", unidadeMed);
                cmd.Parameters.AddWithValue("@EstoqueAtual", estoqueAtual);
                cmd.Parameters.AddWithValue("@PontoPedido", pontoPedido);

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
                string cmdSQL = "UPDATE Insumos SET NomeInsumo = @NomeInsumo, UnidadeMed = @UnidadeMed, EstoqueAtual = @EstoqueAtual, PontoPedido = @PontoPedido WHERE IdInsumo = @IdInsumo";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@IdInsumo", idInsumo);
                cmd.Parameters.AddWithValue("@NomeInsumo", nomeInsumo);
                cmd.Parameters.AddWithValue("@UnidadeMed", unidadeMed);
                cmd.Parameters.AddWithValue("@EstoqueAtual", estoqueAtual);
                cmd.Parameters.AddWithValue("@PontoPedido", pontoPedido);

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
                // Importante: No SQL Server, isso falhará se houver Lotes ou Movimentações vinculadas
                string cmdSQL = "DELETE FROM Insumos WHERE idInsumo = @idInsumo";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@idInsumo", idInsumo);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (SqlException ex)
            {
                // Erro 547 é o código específico do SQL Server para violação de Chave Estrangeira
                if (ex.Number == 547)
                {
                    throw new Exception("Não é possível excluir este insumo pois ele possui lotes, receitas ou movimentações vinculadas.");
                }
                throw new Exception("Erro de banco de dados: " + ex.Message);
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
                string cmdSQL = "SELECT IdInsumo, NomeInsumo, UnidadeMed, EstoqueAtual, PontoPedido FROM Insumos ORDER BY NomeInsumo";

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
                string cmdSQL = "SELECT IdInsumo, NomeInsumo, UnidadeMed, EstoqueAtual, PontoPedido FROM Insumos WHERE IdInsumo = @IdInsumo";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@IdInsumo", idInsumo);

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