using MessageBroker.Sqs.Abstractions;
using Microsoft.Extensions.Hosting;

namespace MessageBroker.Sqs.Consumer
{
    public class SqsConsumerHostedService<TMessage> : IHostedService where TMessage : IMessage
    {
        private readonly ISqsConsumerService<TMessage> _consumerService;

        public SqsConsumerHostedService(ISqsConsumerService<TMessage> consumerService)
        {
            _consumerService = consumerService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _consumerService.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
