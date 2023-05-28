using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using backend_squad1.Models;

namespace backend_squad1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AddMidiaController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public AddMidiaController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files, int chamadoIdChamado)
        {
            try
            {
                if (chamadoIdChamado <= 0)
                {
                    return BadRequest("O ID do chamado deve ser maior que zero.");
                }

                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "fc-services-ba67f-firebase-adminsdk-nytje-1959376e26.json");

                var storage = StorageClient.Create();
                var bucketName = "fc-services-ba67f.appspot.com";

                List<string> urls = new List<string>();

                foreach (var file in files)
                {
                    var filename = $"{DateTime.Now:yyyyMMddHHmmss}-{file.FileName}";
                    var objectName = filename;

                    using (var stream = file.OpenReadStream())
                    {
                        var contentType = file.ContentType;
                        var result = await storage.UploadObjectAsync(bucketName, objectName, contentType, stream);
                    }

                    var url = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(filename)}?alt=media";
                    urls.Add(url);

                    var midia = new Midia
                    {
                        TipoMidia = GetMediaType(file.ContentType),
                        LinkMidia = url,
                        ChamadoIdChamado = chamadoIdChamado
                    };

                    string connectionString = "server=gateway01.us-east-1.prod.aws.tidbcloud.com;port=4000;database=mydb;user=2yztCux73sSBMGV.root;password=A857G3OyIUoJOifl";
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        MySqlCommand command = connection.CreateCommand();
                        command.CommandText = "INSERT INTO Midia (tipoMidia, linkMidia, chamado_idChamado) VALUES (@tipoMidia, @linkMidia, @chamadoIdChamado)";
                        command.Parameters.AddWithValue("@tipoMidia", midia.TipoMidia);
                        command.Parameters.AddWithValue("@linkMidia", midia.LinkMidia);
                        command.Parameters.AddWithValue("@chamadoIdChamado", midia.ChamadoIdChamado);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok(new { urls });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao fazer upload do arquivo: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        private string GetMediaType(string contentType)
        {
            if (contentType.StartsWith("image/"))
            {
                return "Foto";
            }
            else if (contentType.StartsWith("video/"))
            {
                return "Vídeo";
            }
            else
            {
                return "Outros";
            }
        }
    }
}
