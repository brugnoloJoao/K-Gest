using Microsoft.Data.SqlClient;
using System.Data;

namespace K_Gest.BancoDados
{
    public class Insumos
    {
        // --- ATRIBUTOS ---
        public int? idInsumo;
        public string nomeInsumo;
        public string unidadeMed;
        public decimal estoqueAtual;
        public decimal pontoPedido;

        SqlConnection con;

        // --- CONSTRUTOR ---
        public Insumos()
        {
            try
            {
                IConfigurationRoot o_Config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(@".\Configuration\Kantine.json") // Verifique se o caminho do JSON está correto
                    .Build();

                string strConexao = o_Config.GetConnectionString(@"StringConexaoSQLServer");
                con = new SqlConnection(strConexao);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro de configuração de conexão: " + ex.Message);
            }
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

        // --- MÉTODOS CRUD ---

        public void Inserir()
        {
            try
            {
                decimal estoqueCvt = ConverterParaBanco(this.estoqueAtual, this.unidadeMed);
                decimal pontoCvt = ConverterParaBanco(this.pontoPedido, this.unidadeMed);

                string cmdSQL = "INSERT INTO Insumos(NomeInsumo, UnidadeMed, EstoqueAtual, PontoPedido) VALUES(@NomeInsumo, @UnidadeMed, @EstoqueAtual, @PontoPedido)";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@NomeInsumo", nomeInsumo);
                cmd.Parameters.AddWithValue("@UnidadeMed", unidadeMed);
                cmd.Parameters.Add(new SqlParameter("@EstoqueAtual", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = estoqueCvt });
                cmd.Parameters.Add(new SqlParameter("@PontoPedido", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = pontoCvt });

                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { throw new Exception("Erro ao inserir insumo: " + ex.Message); }
            finally { if (con.State == ConnectionState.Open) con.Close(); }
        }

        public void Alterar()
        {
            try
            {
                decimal estoqueCvt = ConverterParaBanco(this.estoqueAtual, this.unidadeMed);
                decimal pontoCvt = ConverterParaBanco(this.pontoPedido, this.unidadeMed);

                string cmdSQL = "UPDATE Insumos SET NomeInsumo = @NomeInsumo, UnidadeMed = @UnidadeMed, EstoqueAtual = @EstoqueAtual, PontoPedido = @PontoPedido WHERE IdInsumo = @IdInsumo";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@IdInsumo", idInsumo);
                cmd.Parameters.AddWithValue("@NomeInsumo", nomeInsumo);
                cmd.Parameters.AddWithValue("@UnidadeMed", unidadeMed);
                cmd.Parameters.Add(new SqlParameter("@EstoqueAtual", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = estoqueCvt });
                cmd.Parameters.Add(new SqlParameter("@PontoPedido", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = pontoCvt });

                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { throw new Exception("Erro ao alterar insumo: " + ex.Message); }
            finally { if (con.State == ConnectionState.Open) con.Close(); }
        }

        public void Excluir()
        {
            try
            {
                string cmdSQL = "DELETE FROM Insumos WHERE IdInsumo = @IdInsumo";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@IdInsumo", idInsumo);

                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) throw new Exception("Não é possível excluir: este insumo está sendo usado em uma receita.");
                throw new Exception("Erro ao excluir: " + ex.Message);
            }
            finally { if (con.State == ConnectionState.Open) con.Close(); }
        }

        public DataTable SelecionarTodos()
        {
            try
            {
                string cmdSQL = "SELECT IdInsumo, NomeInsumo, UnidadeMed, EstoqueAtual, PontoPedido FROM Insumos ORDER BY NomeInsumo";
                SqlDataAdapter da = new SqlDataAdapter(cmdSQL, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Garantimos que retornamos o DataTable mesmo se não houver registros
                return dt;
            }
            catch (Exception ex) { throw new Exception("Erro ao listar insumos: " + ex.Message); }
        }

        public DataTable SelecionarPorID()
        {
            try
            {
                string cmdSQL = "SELECT IdInsumo, NomeInsumo, UnidadeMed, EstoqueAtual, PontoPedido FROM Insumos WHERE IdInsumo = @IdInsumo";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@IdInsumo", idInsumo);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex) { throw new Exception("Erro ao buscar insumo por ID: " + ex.Message); }
        }
    }
}