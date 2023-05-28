using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Authorization;
using backend_squad1.Models;

namespace backend_squad1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CadastroChamadoController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public IActionResult CadastroChamado(CadastrarChamado chamado)
        {
            try
            {
                string connectionString = "server=gateway01.us-east-1.prod.aws.tidbcloud.com;port=4000;database=mydb;user=2yztCux73sSBMGV.root;password=A857G3OyIUoJOifl";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO Chamado (Nome, DataRelato, Descricao, Prioridade, HorarioAbertura, HorarioUltimaAtualizacao, Status, TempoDecorrido, Empregado_Matricula, Tipo) VALUES (@Nome, @DataRelato, @Descricao, @Prioridade, @HorarioAbertura, @HorarioUltimaAtualizacao, @Status, @TempoDecorrido, @Empregado_Matricula, @Tipo); SELECT LAST_INSERT_ID();";

                        command.Parameters.AddWithValue("@Nome", chamado.Nome);
                        command.Parameters.AddWithValue("@Descricao", chamado.Descricao);
                        command.Parameters.AddWithValue("@Prioridade", "Média");
                        command.Parameters.AddWithValue("@HorarioAbertura", DateTime.Now.ToString("HH:mm:ss"));
                        command.Parameters.AddWithValue("@HorarioUltimaAtualizacao", "00:00:00");
                        command.Parameters.AddWithValue("@Status", "Aberto");
                        command.Parameters.AddWithValue("@TempoDecorrido", "00:00:00");
                        command.Parameters.AddWithValue("@Empregado_Matricula", chamado.Empregado_Matricula);
                        command.Parameters.AddWithValue("@Tipo", chamado.Tipo);

                        if (!DateTime.TryParseExact(chamado.DataRelato, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataRelato))
                        {
                            return BadRequest("A data de relato está em um formato inválido.");
                        }

                        chamado.DataRelato = dataRelato.ToString("yyyy/MM/dd");
                        command.Parameters.AddWithValue("@DataRelato", chamado.DataRelato);

                        int id = Convert.ToInt32(command.ExecuteScalar());

                        return Ok(new { Id = id });
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return BadRequest();
            }
        }
    }
}