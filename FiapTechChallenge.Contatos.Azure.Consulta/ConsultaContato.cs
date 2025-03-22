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
        [FunctionName("ConsultaContato")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {           

            if (!int.TryParse(req.Query["ddd"], out int ddd))
            {
                return new BadRequestObjectResult("Parâmetro 'ddd' é obrigatório e deve ser um número válido.");
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

        public record Contato(Guid Id, string Nome, string Telefone, string Email, Guid RegionalId);
        
    }
}
