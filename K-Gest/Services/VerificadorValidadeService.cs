using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using K_Gest.BancoDados;

namespace K_Gest.Services
{
    public class VerificadorValidadeService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<VerificadorValidadeService> _logger;

        // Define o intervalo de tempo (Ex: Rodar a cada 24 horas)
        private readonly TimeSpan _intervalo = TimeSpan.FromHours(24);

        //Para testes utilizar esse
        //private readonly TimeSpan _intervalo = TimeSpan.FromSeconds(10);

        public VerificadorValidadeService(IServiceProvider serviceProvider, ILogger<VerificadorValidadeService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de Verificação de Validade Inicializado.");

            // Loop que mantém o serviço rodando enquanto a aplicação estiver ativa
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Executando rotina de verificação de lotes vencidos...");

                    // Cria um escopo seguro para chamar seu DAO
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        // Instancie o seu DAO aqui. 
                        // Se você usa Injeção de Dependência, mude para: scope.ServiceProvider.GetRequiredService<LoteDAO>();
                        var o_Lote = new Lote();

                        // Chama o método que vai fazer o trabalho pesado no banco
                        int lotesProcessados = o_Lote.ProcessarLotesVencidos();

                        _logger.LogInformation($"Rotina concluída. {lotesProcessados} lotes vencidos foram zerados.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao executar a rotina de lotes vencidos.");
                }

                // Aguarda o tempo determinado antes de rodar o loop novamente
                await Task.Delay(_intervalo, stoppingToken);
            }
        }
    }
}