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
            con.Open();
            SqlTransaction transacao = con.BeginTransaction();

            try
            {
                // 1. Gravar a Venda
                string cmdVenda = "INSERT INTO HistoricoVendas(DataVend, QtdVendida, IdReceita) VALUES(@DataVend, @QtdVendida, @IdReceita)";
                SqlCommand cmd1 = new SqlCommand(cmdVenda, con, transacao);
                cmd1.Parameters.AddWithValue("@DataVend", dataVend);
                cmd1.Parameters.AddWithValue("@QtdVendida", qtdVendida);
                cmd1.Parameters.AddWithValue("@IdReceita", idReceita);
                cmd1.ExecuteNonQuery();

                // 2. Baixa Automática no Estoque (Explosão da Receita)
                // Multiplica a quantidade vendida pelo que é exigido na composição da receita
                string cmdBaixaEstoque = @"
                    UPDATE Insumos 
                    SET estoqueAtual = estoqueAtual - (C.qtdNecessaria * @QtdVendida)
                    FROM Insumos I
                    INNER JOIN Composicao_Receita C ON I.idInsumo = C.idInsumo
                    WHERE C.idReceita = @IdReceita";

                SqlCommand cmd2 = new SqlCommand(cmdBaixaEstoque, con, transacao);
                cmd2.Parameters.AddWithValue("@QtdVendida", qtdVendida);
                cmd2.Parameters.AddWithValue("@IdReceita", idReceita);
                cmd2.ExecuteNonQuery();

                // 3. Gerar Histórico na Movimentação de Estoque
                string cmdMovimentacao = @"
                    INSERT INTO Movimentacao_Estoque (tipoEs, qtdMoviment, motivo, idInsumo)
                    SELECT 'S', (C.qtdNecessaria * @QtdVendida), 'Venda Realizada', C.idInsumo
                    FROM Composicao_Receita C WHERE C.idReceita = @IdReceita";

                SqlCommand cmd3 = new SqlCommand(cmdMovimentacao, con, transacao);
                cmd3.Parameters.AddWithValue("@QtdVendida", qtdVendida);
                cmd3.Parameters.AddWithValue("@IdReceita", idReceita);
                cmd3.ExecuteNonQuery();

                transacao.Commit();
            }
            catch (Exception ex)
            {
                transacao.Rollback();
                throw new Exception("Erro ao processar venda e estoque: " + ex.Message);
            }
            finally { con.Close(); }
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