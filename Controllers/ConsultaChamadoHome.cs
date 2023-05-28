using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Authorization;
using backend_squad1.Models;


namespace backend_squad1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsultaChamadoController : ControllerBase
    {
        [HttpGet("{matricula}", Name = "GetChamadosByMatricula")]
        [Authorize]
        public IActionResult GetAllChamados(int matricula)
        {
            string connectionString = "server=gateway01.us-east-1.prod.aws.tidbcloud.com;port=4000;database=mydb;user=2yztCux73sSBMGV.root;password=A857G3OyIUoJOifl";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = connection.CreateCommand();

            command.CommandText = "SELECT * FROM Chamado WHERE Empregado_Matricula = @Matricula";
            command.Parameters.AddWithValue("@Matricula", matricula);
            connection.Open();
            MySqlDataReader reader = command.ExecuteReader();

            List<ConsultaChamado> chamados = new List<ConsultaChamado>();

            while (reader.Read())
            {
                int idChamado = reader.GetInt32("idChamado");
                string nome = reader.GetString("Nome");
                string dataRelato = reader.GetString("DataRelato");
                string descricao = reader.GetString("Descricao");
                string prioridade = reader.GetString("Prioridade");
                string horarioAbertura = reader.GetString("HorarioAbertura");
                string horarioUltimaAtualizacao = reader.GetString("horarioUltimaAtualizacao");
                string status = reader.GetString("Status");
                string tempoDecorrido = reader.GetString("TempoDecorrido");
                int empregado_Matricula = reader.GetInt32("Empregado_Matricula");
                string tipo = reader.GetString("Tipo");

                ConsultaChamado chamado = new ConsultaChamado
                {
                    idChamado = idChamado,
                    Nome = nome,
                    DataRelato = dataRelato,
                    Descricao = descricao,
                    Prioridade = prioridade,
                    HorarioAbertura = horarioAbertura,
                    HorarioUltimaAtualizacao = horarioUltimaAtualizacao,
                    Status = status,
                    TempoDecorrido = tempoDecorrido,
                    Empregado_Matricula = empregado_Matricula,
                    Tipo = tipo,
                };

                chamados.Add(chamado);
            }

            return Ok(chamados);
        }
    }
}
