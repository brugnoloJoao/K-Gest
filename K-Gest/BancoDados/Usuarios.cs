using Microsoft.Data.SqlClient;
using System.Data;
using K_Gest.Models;

namespace K_Gest.BancoDados
{
    public class Usuarios
    {
        private readonly SqlConnection con;

        public Usuarios()
        {
            IConfigurationRoot o_Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(@".\Configuration\Kantine.json").Build();
            con = new SqlConnection(o_Config.GetConnectionString("StringConexaoSQLServer"));
        }

        /// <summary>
        /// Valida o acesso comparando o login e o HASH da senha recebida (agora em bytes).
        /// </summary>
        public CadastroViewModel? ValidarAcesso(string login, byte[] senhaComHash)
        {
            try
            {
                string sql = @"SELECT nome, perfil FROM Usuarios 
                               WHERE login = @login AND senha = @senha AND ativo = 1";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@senha", senhaComHash); // Passando o array de bytes direto

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    var usuario = new CadastroViewModel
                    {
                        Usuario = login,
                        NomeExibicao = dr["nome"].ToString(),
                    };

                    con.Close();
                    return usuario;
                }

                con.Close();
                return null;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro ao autenticar no banco: " + ex.Message);
            }
        }

        /// <summary>
        /// Insere um novo usuário no banco de dados com a senha criptografada em bytes.
        /// </summary>
        public void Inserir(CadastroViewModel novoUsuario, byte[] senhaHash)
        {
            try
            {
                string sql = @"INSERT INTO Usuarios (login, senha, nome, perfil, ativo) 
                               VALUES (@login, @senha, @nome, @perfil, 1)";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@login", novoUsuario.Usuario);
                cmd.Parameters.AddWithValue("@senha", senhaHash); // Inserindo o byte[] direto no VARBINARY
                cmd.Parameters.AddWithValue("@nome", novoUsuario.NomeExibicao);
                cmd.Parameters.AddWithValue("@perfil", "Usuario");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro ao inserir novo usuário no banco: " + ex.Message);
            }
        }

        /// <summary>
        /// Verifica se um nome de usuário já existe no banco de dados.
        /// </summary>
        public bool ExisteUsuario(string login)
        {
            try
            {
                string sql = "SELECT COUNT(1) FROM Usuarios WHERE login = @login";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@login", login);

                con.Open();
                int resultado = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();

                return resultado > 0;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro ao verificar existência do usuário: " + ex.Message);
            }
        }
    }
}