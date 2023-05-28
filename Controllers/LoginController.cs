using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using backend_squad1.Models;


namespace backend_squad1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult PostLogin([FromBody] LoginRequest login)
        {
            if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Senha))
            {
                return BadRequest("Email e senha são obrigatórios");
            }

            string connectionString = "server=gateway01.us-east-1.prod.aws.tidbcloud.com;port=4000;database=mydb;user=2yztCux73sSBMGV.root;password=A857G3OyIUoJOifl";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = connection.CreateCommand();

            command.CommandText = "SELECT COUNT(*) FROM Empregado WHERE Email = @Email";
            command.Parameters.AddWithValue("@Email", login.Email);
            connection.Open();
            int count = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();

            if (count == 1)
            {
                command.CommandText = "SELECT Senha, Matricula, Nome FROM Empregado WHERE Email = @Email";
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                string hashedPassword = null;
                string matricula = null;
                string nome = null;
                while (reader.Read())
                {
                    hashedPassword = reader["Senha"].ToString();
                    matricula = reader["Matricula"].ToString();
                    nome = reader["Nome"].ToString();
                }
                reader.Close();
                connection.Close();

                string hashedPasswordInput;
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(login.Senha));
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    hashedPasswordInput = builder.ToString();
                }

                if (hashedPasswordInput == hashedPassword)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes("chave-secreta-para-squad1-jwt");
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                    new Claim(ClaimTypes.Name, matricula),
                    new Claim(ClaimTypes.Email, login.Email),
                    new Claim("nome", nome)
                        }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    return Ok(new { Matricula = matricula, Nome = nome, Token = tokenString });
                }
            }
            return BadRequest("Usuário ou senha incorretos");
        }


    }
}
