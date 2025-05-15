using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FiapTechChallenge.Contatos.Azure.Consulta
{
    public static class ConsultaContato
    {
        [FunctionName("ContatoPorDDD")]
        public static async Task<IActionResult> RunDdd(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ContatoPorDDD/{ddd}")] HttpRequest req,
            string ddd, ILogger log)
        {

            if (!int.TryParse(ddd, out int Ddd))
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

                var contatos = await db.QueryAsync<Contato>(sql, new { DDD = Ddd });

                if(contatos.Count() == 0)
                {
                    return new BadRequestObjectResult("Nenhum contato encontrato");
                }
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ContatoPorId/{id}")] HttpRequest req,
            string id, ILogger log)
        {

            if (!Guid.TryParse(id, out Guid Id))
            {
                return new BadRequestObjectResult("Parâmetro 'Id' é obrigatório e deve ser um Guid válido.");
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

                if (contato.Count() == 0)
                {
                    return new BadRequestObjectResult("Nenhum contato encontrato");
                }
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
