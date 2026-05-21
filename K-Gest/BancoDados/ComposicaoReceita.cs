using K_Gest.Models;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
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
        public string unidadeExibicao;
        SqlConnection con;

        public ComposicaoReceita()
        {
            IConfigurationRoot o_Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(@".\Configuration\Kantine.json").Build();
            con = new SqlConnection(o_Config.GetConnectionString(@"StringConexaoSQLServer"));
        }

        public void InserirFichaTecnica(int idR, List<ItemComposicao> itens)
        {
            con.Open();
            SqlTransaction tr = con.BeginTransaction();
            try
            {
                foreach (var i in itens)
                {
                    string sql = "INSERT INTO Composicao_Receita (qtdNecessaria, idReceita, idInsumo) VALUES(@q,@r,@i)";
                    SqlCommand cmd = new SqlCommand(sql, con, tr);
                    cmd.Parameters.AddWithValue("@q", i.Quantidade);
                    cmd.Parameters.AddWithValue("@r", idR);
                    cmd.Parameters.AddWithValue("@i", i.IdInsumo);
                    cmd.ExecuteNonQuery();
                }
                tr.Commit();
            }
            catch { tr.Rollback(); throw; }
            finally { con.Close(); }
        }

        public void Excluir()
        {
            string sql = "DELETE FROM Composicao_Receita WHERE idComposicao = @id";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", idComposicao);
            con.Open(); cmd.ExecuteNonQuery(); con.Close();
        }

        public DataTable SelecionarPorReceita(int idReceita)
        {
            try
            {
                string cmdSQL = @"SELECT 
                            C.idComposicao, 
                            C.idInsumo, 
                            I.nomeInsumo, 
                            C.qtdNecessaria, 
                            C.unidadeExibicao
                          FROM Composicao_Receita C
                          INNER JOIN Insumos I ON C.idInsumo = I.idInsumo
                          WHERE C.idReceita = @id";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@id", idReceita);
                DataTable dt = new DataTable();
                o_DataAdapter.Fill(dt);
                return dt;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public void ExcluirIngredienteIndividual(int id)
        {
            string sql = "DELETE FROM Composicao_Receita WHERE idComposicao = @id";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public DataTable SelecionarAgrupado()
        {
            string sql = @"SELECT idReceita, nomePrato FROM Receitas ORDER BY nomePrato";
            SqlDataAdapter da = new SqlDataAdapter(sql, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public void Alterar()
        {
            string sql = "UPDATE Composicao_Receita SET qtdNecessaria=@q, idReceita=@r, idInsumo=@i, unidadeExibicao=@u WHERE idComposicao=@c, ";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@c", idComposicao);
            cmd.Parameters.AddWithValue("@q", qtdNecessaria);
            cmd.Parameters.AddWithValue("@u", unidadeExibicao);
            cmd.Parameters.AddWithValue("@r", idReceita);
            cmd.Parameters.AddWithValue("@i", idInsumo);
            con.Open(); cmd.ExecuteNonQuery(); con.Close();
        }

        public void AtualizarFichaTecnica(int idReceita, List<ItemComposicao> itens)
        {
            con.Open();
            SqlTransaction trans = con.BeginTransaction();
            try
            {
                string delSQL = "DELETE FROM Composicao_Receita WHERE idReceita = @idR";
                SqlCommand cmdDel = new SqlCommand(delSQL, con, trans);
                cmdDel.Parameters.AddWithValue("@idR", idReceita);
                cmdDel.ExecuteNonQuery();

                foreach (var item in itens)
                {
                    string insSQL = "INSERT INTO Composicao_Receita (qtdNecessaria,unidadeExibicao, idReceita, idInsumo) VALUES(@qtd, @u, @idR, @idI)";
                    SqlCommand cmdIns = new SqlCommand(insSQL, con, trans);
                    cmdIns.Parameters.AddWithValue("@qtd", item.Quantidade);
                    cmdIns.Parameters.AddWithValue("@idR", idReceita);
                    cmdIns.Parameters.AddWithValue("@idI", item.IdInsumo);
                    cmdIns.Parameters.AddWithValue("@u", item.UnidadeExibicao);
                    cmdIns.ExecuteNonQuery();
                }
                trans.Commit();
            }
            catch { trans.Rollback(); throw; }
            finally { con.Close(); }
        }

        public DataTable SelecionarPorID()
        {
            string sql = "SELECT * FROM Composicao_Receita WHERE idComposicao = @id";
            SqlDataAdapter da = new SqlDataAdapter(sql, con);
            da.SelectCommand.Parameters.AddWithValue("@id", idComposicao);
            DataTable dt = new DataTable(); da.Fill(dt); return dt;
        }
    }
}