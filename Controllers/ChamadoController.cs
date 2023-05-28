using backend_squad1.Constants.Sql;
using backend_squad1.Data;
using backend_squad1.Models.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_squad1.Controllers;

[Authorize]
public partial class ChamadoController: ControllerBase
{
    private readonly IAppDbConnection db;

    public ChamadoController(IAppDbConnection db) {
        this.db = db;
    }

    [HttpPost]
    public IActionResult AdicionarComentario([FromBody] AdicionarComentarioInput input) {

        try  {
            this.Validate(input);
            int? comentarioId = null;

            try {
                this.db.Open();
                comentarioId = this.db.ExecuteScalar<int>(
                    new AppDbModels.ExecuteScalar {
                        QuerySql = ChamadoSql.CreateComentarioSql,
                        parameters = AppDbModels.AppDbParameters.GetInstance()
                            .add("@idChamado", input.idChamado)
                            .add("@matricula", input.matricula)
                            .add("@texto", input.texto)
                    }
                );

                this.db.Commit();
            }

            catch (Exception error) {
                Console.WriteLine(error.Message);
                this.db.Rollback();
            }
            
            finally {
                this.db.Close();
            }

            return Ok(new { ComentarioId = comentarioId });
        }

        catch (Exception error) {
            Console.WriteLine(error.Message);
            return BadRequest();
        }
    }
}