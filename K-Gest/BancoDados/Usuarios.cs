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

        public LoginViewModel? ValidarAcesso(string login, string senha)
        {
            try
            {
                string sql = @"SELECT nome, perfil FROM Usuarios 
                               WHERE login = @login AND senha = @senha AND ativo = 1";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@senha", senha);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    var usuario = new LoginViewModel
                    {
                        Usuario = login,
                        NomeExibicao = dr["nome"].ToString(),
                        Perfil = dr["perfil"].ToString()
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

    }
}