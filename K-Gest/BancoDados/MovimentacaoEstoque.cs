using Microsoft.Data.SqlClient;
using System.Data;

namespace K_Gest.BancoDados
{
    public class MovimentacaoEstoque
    {
        //-------------------------------------------------------------
        // Atributos
        //-------------------------------------------------------------
        public int? idEstoque;
        public string tipoEs;
        public int qntdEstoque;
        public string motivo;
        public int idInsumo;

        SqlConnection con;

        //-------------------------------------------------------------
        // Construtor
        //-------------------------------------------------------------
        public MovimentacaoEstoque()
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
                string cmdSQL = "INSERT INTO MovimentacaoEstoque(TipoEs, QntdEstoque, Motivo, IdInsumo) VALUES(@TipoEs, @QntdEstoque, @Motivo, @IdInsumo)";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@TipoEs", tipoEs);
                cmd.Parameters.AddWithValue("@QntdEstoque", qntdEstoque);
                cmd.Parameters.AddWithValue("@Motivo", motivo);
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
                string cmdSQL = "UPDATE MovimentacaoEstoque SET TipoEs = @TipoEs, QntdEstoque = @QntdEstoque, Motivo = @Motivo, IdInsumo = @IdInsumo WHERE IdEstoque = @IdEstoque";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@IdEstoque", idEstoque);
                cmd.Parameters.AddWithValue("@TipoEs", tipoEs);
                cmd.Parameters.AddWithValue("@QntdEstoque", qntdEstoque);
                cmd.Parameters.AddWithValue("@Motivo", motivo);
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
                string cmdSQL = "DELETE FROM MovimentacaoEstoque WHERE IdEstoque = @IdEstoque";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@IdEstoque", idEstoque);

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
                // SQL com JOIN para trazer o Nome do Insumo
                string cmdSQL = @"SELECT 
                                    M.IdEstoque, 
                                    M.TipoEs, 
                                    M.QntdEstoque, 
                                    M.Motivo, 
                                    I.Nome AS NomeInsumo 
                                  FROM MovimentacaoEstoque M
                                  INNER JOIN Insumos I ON M.IdInsumo = I.IdInsumo
                                  ORDER BY M.IdEstoque DESC";

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
                                    M.IdEstoque, 
                                    M.TipoEs, 
                                    M.QntdEstoque, 
                                    M.Motivo, 
                                    M.IdInsumo,
                                    I.Nome AS NomeInsumo 
                                  FROM MovimentacaoEstoque M
                                  INNER JOIN Insumos I ON M.IdInsumo = I.IdInsumo
                                  WHERE M.IdEstoque = @IdEstoque";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@IdEstoque", idEstoque);

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