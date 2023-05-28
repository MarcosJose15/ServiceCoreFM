using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using backend_squad1.Models;

namespace backend_squad1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CadastrarUsuarioController : ControllerBase
    {
        [HttpPost(Name = "Adicionar Usuario")]
        public IActionResult PostCadastrarEmpregado([FromBody] Empregado user)
        {
            string connectionString = "server=gateway01.us-east-1.prod.aws.tidbcloud.com;port=4000;database=mydb;user=2yztCux73sSBMGV.root;password=A857G3OyIUoJOifl";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = connection.CreateCommand();

            command.CommandText = "SELECT COUNT(*) FROM Empregado WHERE Matricula = @Matricula";
            command.Parameters.AddWithValue("@Matricula", user.Matricula);
            command.CommandText = "SELECT COUNT(*) FROM Empregado WHERE email = @Email";
            command.Parameters.AddWithValue("@Email", user.Email);
            connection.Open();
            int count = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();

            if (count > 0)
            {
                return BadRequest("Usuário já cadastrado");
            }

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Senha))
            {
                return BadRequest("Email e senha são obrigatórios");
            }

            connection.Open();
            int count1 = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            if (count1 > 0)
            {
                // Email já cadastrado
            }
            else
            {
                // Email ainda não cadastrado
            }

            string hashedPassword;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(user.Senha));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                hashedPassword = builder.ToString();
            }

            command.CommandText = "INSERT INTO Empregado (Matricula, Nome, Funcao, Email, Senha, Resolutor, Setor_idSetor, Filial_idFilial) VALUES (@Matricula, @Nome, @Funcao, @Email, @Senha, @Resolutor, @Setor_idSetor, @Filial_idFilial)";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@Matricula", user.Matricula);
            command.Parameters.AddWithValue("@Nome", user.Nome);
            command.Parameters.AddWithValue("@Funcao", user.Funcao);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Senha", hashedPassword);
            command.Parameters.AddWithValue("@Resolutor", user.Resolutor);
            command.Parameters.AddWithValue("@Setor_idSetor", user.Setor_idSetor);
            command.Parameters.AddWithValue("@Filial_idFilial", user.Filial_idFilial);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            return Ok();
        }
    }
}
