using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Mail;

namespace backend_squad1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FluxoRecuperarSenhaController : ControllerBase
    {
        private const string connectionString = "server=gateway01.us-east-1.prod.aws.tidbcloud.com;port=4000;database=mydb;user=2yztCux73sSBMGV.root;password=A857G3OyIUoJOifl";
        private const string emailFrom = "pixelsquadfcx@outlook.com";
        private const string emailPassword = "pixelsquad1";

        [HttpGet("verificar-usuario/{matricula}", Name = "Verificar Usuário Matricula")]
        public IActionResult VerificarUsuarioMatricula(string matricula)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT Matricula FROM Empregado WHERE Matricula = @Matricula";
                command.Parameters.AddWithValue("@Matricula", matricula);
                connection.Open();
                object result = command.ExecuteScalar();
                if (result == null)
                {
                    return NotFound("Usuário não encontrado");
                }
            }

            return Ok("Usuário encontrado");
        }


        [HttpGet("verificar-usuario/{matricula}/{email}", Name = "Verificar Usuário Email")]
        public IActionResult VerificarUsuarioEmail(string matricula, string email)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT Email FROM Empregado WHERE Matricula = @Matricula";
                command.Parameters.AddWithValue("@Matricula", matricula);
                connection.Open();
                object result = command.ExecuteScalar();
                if (result == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                string emailNoBanco = result.ToString();
                if (email == emailNoBanco)
                {
                    return BadRequest("O email fornecido é igual ao email registrado no banco de dados");
                }
            }

            return Ok("Usuário encontrado");
        }


        [HttpGet("enviar-codigo/{matricula}/{email}", Name = "Verificar E-mail")]
        public IActionResult VerificarEmail(string matricula, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("O email não pode ser vazio ou nulo");
            }

            string emailAssociado = ObterEmailPorMatricula(matricula);

            if (emailAssociado == null)
            {
                return BadRequest("A matrícula informada não está associada a este email");
            }

            if (email != emailAssociado)
            {
                return BadRequest("O email informado não corresponde ao email associado à matrícula");
            }

            string codigoRecuperacao = GerarCodigoRecuperacao();
            bool emailEnviado = EnviarCodigoRecuperacaoPorEmail(email, codigoRecuperacao);

            if (!emailEnviado)
            {
                return StatusCode(500, "Não foi possível enviar o email de recuperação");
            }

            // Atualizar a coluna codigoRecuperacao na tabela Empregado
            AtualizarCodigoRecuperacao(matricula, codigoRecuperacao);

            return Ok("Email com código de recuperação enviado");
        }

        [HttpGet("verificar-codigo/{matricula}/{codigo}", Name = "Verificar Código")]
        public IActionResult VerificarCodigo(string matricula, string codigo)
        {
            string codigoRecuperacao = ObterCodigoRecuperacao(matricula);

            if (codigoRecuperacao == null)
            {
                return BadRequest("Matrícula inválida ou código de recuperação não encontrado");
            }

            if (codigoRecuperacao != codigo)
            {
                return BadRequest("Código de recuperação incorreto");
            }

            return Ok("Código de recuperação válido");
        }

        [HttpPut("alterar-senha/{matricula}", Name = "Alterar Senha")]
        public IActionResult AlterarSenha(string matricula, [FromBody] AlterarSenhaRequest request)
        {
            string codigoRecuperacao = ObterCodigoRecuperacao(matricula);

            if (codigoRecuperacao == null)
            {
                return BadRequest("Matrícula inválida ou código de recuperação não encontrado");
            }

            if (codigoRecuperacao != request.CodigoRecuperacao)
            {
                return BadRequest("Código de recuperação incorreto");
            }

            if (request.NovaSenha != request.ConfirmacaoSenha)
            {
                return BadRequest("A nova senha e a confirmação de senha não correspondem");
            }

            string novaSenha = request.NovaSenha;

            // Alterar a senha do usuário
            AlterarSenhaNoBancoDeDados(matricula, novaSenha);

            // Gerar um novo código de recuperação aleatório
            string novoCodigoRecuperacao = GerarCodigoRecuperacao();

            // Atualizar o código de recuperação no banco de dados
            AtualizarCodigoRecuperacao(matricula, novoCodigoRecuperacao);

            return Ok("Senha alterada com sucesso");
        }

        private string ObterEmailPorMatricula(string matricula)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT Email FROM Empregado WHERE Matricula = @Matricula";
                command.Parameters.AddWithValue("@Matricula", matricula);
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    return result.ToString();
                }
            }

            return null;
        }

        private string GerarCodigoRecuperacao()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private bool EnviarCodigoRecuperacaoPorEmail(string email, string codigoRecuperacao)
        {
            string subject = "Recuperação de Senha";
            string body = $"Seu código de recuperação é: {codigoRecuperacao}";

            try
            {
                using (var smtpClient = new SmtpClient("smtp-mail.outlook.com", 587))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(emailFrom, emailPassword);
                    smtpClient.EnableSsl = true;

                    var mailMessage = new MailMessage()
                    {
                        From = new MailAddress(emailFrom),
                        To = { new MailAddress(email) },
                        Subject = subject,
                        Body = body
                    };

                    smtpClient.Send(mailMessage);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar o e-mail: {ex.Message}");
                return false;
            }
        }

        private void AtualizarCodigoRecuperacao(string matricula, string codigoRecuperacao)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE Empregado SET codigoRecuperacao = @CodigoRecuperacao WHERE Matricula = @Matricula";
                command.Parameters.AddWithValue("@CodigoRecuperacao", codigoRecuperacao);
                command.Parameters.AddWithValue("@Matricula", matricula);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private string ObterCodigoRecuperacao(string matricula)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT codigoRecuperacao FROM Empregado WHERE Matricula = @Matricula";
                command.Parameters.AddWithValue("@Matricula", matricula);
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    return result.ToString();
                }
            }

            return null;
        }

        private void AlterarSenhaNoBancoDeDados(string matricula, string novaSenha)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE Empregado SET Senha = @Senha WHERE Matricula = @Matricula";
                command.Parameters.AddWithValue("@Senha", novaSenha);
                command.Parameters.AddWithValue("@Matricula", matricula);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }

    public class AlterarSenhaRequest
    {
        public string NovaSenha { get; set; }
        public string ConfirmacaoSenha { get; set; }
        public string CodigoRecuperacao { get; set; }
    }
}
