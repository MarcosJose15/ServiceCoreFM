using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Authorization;
using backend_squad1.Models;

namespace backend_squad1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlterarChamadoController : ControllerBase
    {
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult AlterarChamado(int id, CadastrarChamado chamado)
        {
            string connectionString = "server=gateway01.us-east-1.prod.aws.tidbcloud.com;port=4000;database=mydb;user=2yztCux73sSBMGV.root;password=A857G3OyIUoJOifl";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = connection.CreateCommand();

            command.CommandText = "UPDATE Chamado SET Nome = @Nome, DataRelato = @DataRelato, Descricao = @Descricao, Prioridade = @Prioridade, HorarioAbertura = @HorarioAbertura, HorarioUltimaAtualizacao = @HorarioUltimaAtualizacao, Status = @Status, TempoDecorrido = @TempoDecorrido, Empregado_Matricula = @Empregado_Matricula, Tipo = @Tipo WHERE Id = @Id";

            command.Parameters.AddWithValue("@Nome", chamado.Nome);
            command.Parameters.AddWithValue("@DataRelato", chamado.DataRelato);
            command.Parameters.AddWithValue("@Descricao", chamado.Descricao);
            // command.Parameters.AddWithValue("@Prioridade", chamado.Prioridade);
            command.Parameters.AddWithValue("@HorarioAbertura", "00:00:00");
            command.Parameters.AddWithValue("@HorarioUltimaAtualizacao", "00:00:00");
            // command.Parameters.AddWithValue("@Status", chamado.Status);
            command.Parameters.AddWithValue("@TempoDecorrido", "00:00:00");
            command.Parameters.AddWithValue("@Empregado_Matricula", chamado.Empregado_Matricula);
            command.Parameters.AddWithValue("@Tipo", chamado.Tipo);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            if (!DateTime.TryParseExact(chamado.DataRelato, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataRelato))
            {
                return BadRequest("A data de relato está em um formato inválido.");
            }


            int rowsAffected = command.ExecuteNonQuery();
            connection.Close();

            if (rowsAffected > 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
