using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FiapTechChallenge.Contatos.Azure.Consulta
{
    public static class TrataDeadLetterQueue
    {
        [FunctionName("TrataDeadLetterQueue")]
        public static void Run(
            [RabbitMQTrigger("x.contato.deadletter", ConnectionStringSetting = "RABBITMQ_CONNECTION")] string inputMessage,
            ILogger log)
        {
            log.LogInformation($"Mensagem obtida da fila DeadLetter: {inputMessage}");
        }
    }
}
