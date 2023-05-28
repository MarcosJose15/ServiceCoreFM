using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Authorization;

namespace backend_squad1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExcluirChamadoController : ControllerBase
    {
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult ExcluirChamado(int id)
        {
            string connectionString = "server=gateway01.us-east-1.prod.aws.tidbcloud.com;port=4000;database=mydb;user=2yztCux73sSBMGV.root;password=A857G3OyIUoJOifl";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = connection.CreateCommand();

            command.CommandText = "DELETE FROM Chamado WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
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
