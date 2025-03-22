using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;

namespace FiapTechChallenge.Contatos.Azure.Consulta
{
    public static class ConsultaContato
    {
        [FunctionName("ContatoPorDDD")]
        public static async Task<IActionResult> RunDdd(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {           

            if (!int.TryParse(req.Query["ddd"], out int ddd))
            {
                return new BadRequestObjectResult("Par�metro 'ddd' � obrigat�rio e deve ser um n�mero v�lido.");
            }

            try
            {
                using IDbConnection db = new NpgsqlConnection(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION"));
                string sql = @"
                            SELECT c.""Id"", 
                                   c.""Nome"", 
                                   c.""Telefone"", 
                                   c.""Email"", 
                                   c.""RegionalId"" 
                              FROM ""Contato"" c JOIN ""Regional"" r
                                ON c.""RegionalId"" = r.""Id""
                             WHERE r.""Ddd"" = @DDD";

                var contatos = await db.QueryAsync<Contato>(sql, new { DDD = ddd });

                return new OkObjectResult(contatos);
            }
            catch (Exception ex)
            {
                log.LogError($"Erro ao acessar o banco: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("ContatoPorId")]
        public static async Task<IActionResult> RunId(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            if (!Guid.TryParse(req.Query["id"], out Guid Id))
            {
                return new BadRequestObjectResult("Par�metro 'Id' � obrigat�rio e deve ser um Guid v�lido.");
            }

            try
            {
                using IDbConnection db = new NpgsqlConnection(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION"));
                string sql = @"
                            SELECT c.""Id"", 
                                   c.""Nome"", 
                                   c.""Telefone"", 
                                   c.""Email"", 
                                   c.""RegionalId"" 
                              FROM ""Contato"" c 
                             WHERE c.""Id"" = @ID";

                var contato = await db.QueryAsync<Contato>(sql, new { ID = Id });

                return new OkObjectResult(contato);
            }
            catch (Exception ex)
            {
                log.LogError($"Erro ao acessar o banco: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }

        public record Contato(Guid Id, string Nome, string Telefone, string Email, Guid RegionalId);
        
    }
}
