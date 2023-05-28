using System;
using System.IO;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using backend_squad1;
using System.Data;

namespace backend_squad1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RemoveMidiaController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public RemoveMidiaController(IWebHostEnvironment env)
        {
            _env = env;
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
            string connectionString = "server=gateway01.us-east-1.prod.aws.tidbcloud.com;port=4000;database=mydb;user=2yztCux73sSBMGV.root;password=A857G3OyIUoJOifl";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Exclui o registro da mídia do banco de dados
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Midia WHERE idMidia = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao excluir a mídia: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }


    }
}
