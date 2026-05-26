using Microsoft.Data.SqlClient;
using System.Data;

namespace K_Gest.BancoDados
{
    public class MovimentacaoEstoque
    {
        public int? idEstoque;
        public string tipoEs; // "E" para Entrada, "S" para Saída
        public decimal qtdMoviment;
        public string motivo;
        public int idInsumo;

        SqlConnection con;

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
        // INSERIR COM ATUALIZAÇÃO DE ESTOQUE
        //-------------------------------------------------------------
        public void Inserir(string unidadeSelecionada)
        {
            con.Open();
            SqlTransaction tran = con.BeginTransaction();

            try
            {
                // 1. Calculamos o valor convertido para somar/subtrair no estoque principal
                decimal qtdReal = ConverterParaBanco(this.qtdMoviment, unidadeSelecionada);

                // 2. Registra o histórico na tabela Movimentacao_Estoque
                string cmdMovimentacao = @"INSERT INTO Movimentacao_Estoque(tipoEs, qtdMoviment, motivo, idInsumo) 
                                         VALUES(@tipoEs, @qtdMoviment, @motivo, @idInsumo)";

                SqlCommand cmd1 = new SqlCommand(cmdMovimentacao, con, tran);
                cmd1.Parameters.AddWithValue("@tipoEs", tipoEs);
                cmd1.Parameters.AddWithValue("@qtdMoviment", qtdReal); // Salva o valor original (ex: 10)
                cmd1.Parameters.AddWithValue("@motivo", motivo);
                cmd1.Parameters.AddWithValue("@idInsumo", idInsumo);
                cmd1.ExecuteNonQuery();

                // 3. Atualiza a tabela Insumos com o valor CONVERTIDO (ex: 10000)
                string operacao = (tipoEs.ToUpper() == "E") ? "+" : "-";
                string cmdInsumo = $"UPDATE Insumos SET estoqueAtual = estoqueAtual {operacao} @qtdReal WHERE idInsumo = @idInsumo";

                SqlCommand cmd2 = new SqlCommand(cmdInsumo, con, tran);

                // Parâmetro tipado para evitar estouro aritmético
                SqlParameter paramQtd = new SqlParameter("@qtdReal", SqlDbType.Decimal);
                paramQtd.Precision = 18;
                paramQtd.Scale = 2;
                paramQtd.Value = qtdReal;

                cmd2.Parameters.Add(paramQtd);
                cmd2.Parameters.AddWithValue("@idInsumo", idInsumo);
                cmd2.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw new Exception("Erro ao processar movimentação: " + ex.Message);
            }
            finally { con.Close(); }
        }
        public void InserirPorLote()
        {
            con.Open();
            SqlTransaction tran = con.BeginTransaction();

            try
            {

                // Registra o histórico na tabela Movimentacao_Estoque
                string cmdMovimentacao = @"INSERT INTO Movimentacao_Estoque(tipoEs, qtdMoviment, motivo, idInsumo) 
                                         VALUES(@tipoEs, @qtdMoviment, @motivo, @idInsumo)";

                SqlCommand cmd1 = new SqlCommand(cmdMovimentacao, con, tran);
                cmd1.Parameters.AddWithValue("@tipoEs", tipoEs);
                cmd1.Parameters.AddWithValue("@qtdMoviment", qtdMoviment); 
                cmd1.Parameters.AddWithValue("@motivo", motivo);
                cmd1.Parameters.AddWithValue("@idInsumo", idInsumo);
                cmd1.ExecuteNonQuery();

                // Atualiza a tabela Insumos com o valor CONVERTIDO (ex: 10000)
                string operacao = (tipoEs.ToUpper() == "E") ? "+" : "-";
                string cmdInsumo = $"UPDATE Insumos SET estoqueAtual = estoqueAtual {operacao} @qtdReal WHERE idInsumo = @idInsumo";

                SqlCommand cmd2 = new SqlCommand(cmdInsumo, con, tran);

                // Parâmetro tipado para evitar estouro aritmético
                SqlParameter paramQtd = new SqlParameter("@qtdReal", SqlDbType.Decimal);
                paramQtd.Precision = 18;
                paramQtd.Scale = 2;
                paramQtd.Value = qtdMoviment;

                cmd2.Parameters.Add(paramQtd);
                cmd2.Parameters.AddWithValue("@idInsumo", idInsumo);
                cmd2.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw new Exception("Erro ao processar movimentação: " + ex.Message);
            }
            finally { con.Close(); }
        }

         // --- MÉTODOS DE CONVERSÃO ---

        public decimal ConverterParaBanco(decimal valor, string unidade)
        {
            if (string.IsNullOrEmpty(unidade)) return valor;
            switch (unidade.ToUpper())
            {
                case "KG":
                case "L":
                    return valor * 1000; // Converte para gramas ou mililitros
                default:
                    return valor;
            }
        }

        public decimal ConverterParaTela(decimal valor, string unidade)
        {
            if (string.IsNullOrEmpty(unidade)) return valor;
            switch (unidade.ToUpper())
            {
                case "KG":
                case "L":
                    return valor / 1000; // Converte de volta para KG ou L
                default:
                    return valor;
            }
        }

        //-------------------------------------------------------------
        // MÉTODOS PARA O DASHBOARD E LISTA DE COMPRAS
        //-------------------------------------------------------------

        public DataTable SelecionarPorMotivo(string motivoFiltro)
        {
            try
            {
                // SQL com JOIN para saber qual insumo foi desperdiçado
                string cmdSQL = @"SELECT M.IdEstoque, M.TipoEs, M.QtdMoviment, M.Motivo, I.Nome as NomeInsumo 
                          FROM MovimentacaoEstoque M
                          INNER JOIN Insumos I ON M.IdInsumo = I.IdInsumo
                          WHERE M.Motivo LIKE @Motivo
                          ORDER BY M.IdEstoque DESC";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                // O % faz com que ele busque qualquer frase que contenha a palavra (Ex: "Desperdício de sobra")
                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@Motivo", "%" + motivoFiltro + "%");

                con.Open();
                DataTable dtPesquisa = new DataTable();
                int qtdeLinhas = o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                return qtdeLinhas > 0 ? dtPesquisa : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao filtrar motivo: " + ex.Message);
            }
        }

        public DataTable SelecionarTodos()
        {
            try
            {
                string cmdSQL = @"SELECT M.idEstoque, M.tipoEs, M.qtdMoviment, M.motivo, 
                                 I.nomeInsumo, I.unidadeMed, M.idInsumo
                          FROM Movimentacao_Estoque M
                          INNER JOIN Insumos I ON M.idInsumo = I.idInsumo
                          ORDER BY M.idEstoque DESC";

                SqlDataAdapter da = new SqlDataAdapter(cmdSQL, con);
                DataTable dt = new DataTable();
                con.Open();
                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public DataTable SelecionarEntradas()
        {
            try
            {
                string cmdSQL = @"SELECT M.idEstoque, M.tipoEs, M.qtdMoviment, M.motivo, 
                                 I.nomeInsumo, I.unidadeMed, M.idInsumo
                          FROM Movimentacao_Estoque M
                          INNER JOIN Insumos I ON M.idInsumo = I.idInsumo
                          WHERE M.tipoEs = 'E'
                          ORDER BY M.idEstoque DESC";

                SqlDataAdapter da = new SqlDataAdapter(cmdSQL, con);
                DataTable dt = new DataTable();
                con.Open();
                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public DataTable SelecionarSaidas()
        {
            try
            {
                string cmdSQL = @"SELECT M.idEstoque, M.tipoEs, M.qtdMoviment, M.motivo, 
                                 I.nomeInsumo, I.unidadeMed, M.idInsumo
                          FROM Movimentacao_Estoque M
                          INNER JOIN Insumos I ON M.idInsumo = I.idInsumo
                          WHERE M.tipoEs = 'S'
                          ORDER BY M.idEstoque DESC";

                SqlDataAdapter da = new SqlDataAdapter(cmdSQL, con);
                DataTable dt = new DataTable();
                con.Open();
                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public DataTable SelecionarPorID()
        {
            try
            {
                string cmdSQL = "SELECT * FROM Movimentacao_Estoque WHERE idEstoque = @idEstoque";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@idEstoque", idEstoque);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                con.Open();
                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public void Excluir()
        {
            con.Open();
            SqlTransaction tran = con.BeginTransaction();

            try
            {
                
                // Precisamos saber se era Entrada ou Saída para devolver ao estoque corretamente
                string sqlBusca = "SELECT tipoEs, qtdMoviment, idInsumo FROM Movimentacao_Estoque WHERE idEstoque = @idEstoque";
                SqlCommand cmdBusca = new SqlCommand(sqlBusca, con, tran);
                cmdBusca.Parameters.AddWithValue("@idEstoque", idEstoque);

                SqlDataReader dr = cmdBusca.ExecuteReader();
                dr.Read();

                // Atribuímos às variáveis locais para garantir que temos os valores do BD
                string v_tipo = dr["tipoEs"].ToString();
                int v_qtd = Convert.ToInt32(dr["qtdMoviment"]);
                int v_idInsumo = Convert.ToInt32(dr["idInsumo"]);

                dr.Close();

                //  Lógica de Estorno
                string opEstorno = (v_tipo.ToUpper() == "E") ? "-" : "+";

                // Aqui usamos nomes de parâmetros que remetem aos seus atributos
                string sqlEstorno = $"UPDATE Insumos SET estoqueAtual = estoqueAtual {opEstorno} @qtdMoviment WHERE idInsumo = @idInsumo";
                SqlCommand cmdEstorno = new SqlCommand(sqlEstorno, con, tran);

                cmdEstorno.Parameters.AddWithValue("@qtdMoviment", v_qtd);
                cmdEstorno.Parameters.AddWithValue("@idInsumo", v_idInsumo);
                cmdEstorno.ExecuteNonQuery();

               
                string sqlDel = "DELETE FROM Movimentacao_Estoque WHERE idEstoque = @idEstoque";
                SqlCommand cmdDel = new SqlCommand(sqlDel, con, tran);
                cmdDel.Parameters.AddWithValue("@idEstoque", idEstoque);
                cmdDel.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw new Exception("Erro ao excluir e estornar estoque: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
    }
}