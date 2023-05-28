using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Authorization;
using backend_squad1.Models;
using backend_squad1.Constants.Sql;
using backend_squad1.Data;
using backend_squad1.Models.Inputs;

namespace fc_services_core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FiltrarChamadosController : ControllerBase
    {
        private readonly IAppDbConnection db;

        public FiltrarChamadosController(IAppDbConnection db)
        {
            this.db = db;
        }

        [HttpPost]
        [Authorize]
        public IActionResult FiltrarChamados(FiltroChamados filtro)
        {
            try
            {
                this.db.Open();

                string query = "SELECT * FROM chamado WHERE ";

                List<MySqlParameter> parameters = new List<MySqlParameter>();

                if (filtro.DataRelato != null)
                {
                    query += "datarelato = @datarelato AND ";
                    parameters.Add(new MySqlParameter("@datarelato", filtro.DataRelato));
                }

                if (filtro.Prioridade != null)
                {
                    query += "prioridade = @prioridade AND ";
                    parameters.Add(new MySqlParameter("@prioridade", filtro.Prioridade));
                }

                if (filtro.Status != null)
                {
                    query += "status = @status AND ";
                    parameters.Add(new MySqlParameter("@status", filtro.Status));
                }

                if (filtro.TempoDecorrido != null)
                {
                    query += "tempodecorrido = @tempodecorrido AND ";
                    parameters.Add(new MySqlParameter("@tempodecorrido", filtro.TempoDecorrido));
                }

                if (filtro.Empregado_Matricula != null)
                {
                    query += "empregado_matricula = @empregado_matricula AND ";
                    parameters.Add(new MySqlParameter("@empregado_matricula", filtro.Empregado_Matricula));
                }

                if (filtro.Tipo != null)
                {
                    query += "tipo = @tipo AND ";
                    parameters.Add(new MySqlParameter("@tipo", filtro.Tipo));
                }

                // Remove o último "AND" da consulta
                query = query.TrimEnd(' ', 'A', 'N', 'D');

                var chamados = this.db.Query<ConsultaChamadoID>(new AppDbModels.Query
                {
                    QuerySql = query,
                    Parameters = parameters.ToArray()
                });

                return Ok(chamados);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return BadRequest();
            }
            finally
            {
                this.db.Close();
            }
        }
    }
}