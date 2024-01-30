using MessageBroker.Sqs.Abstractions;
using WebApplication1.Messages;

namespace Handlers
{
    public class FirstHandler : IMessageHandler
    {
        private readonly ILogger<FirstHandler> _logger;

        public FirstHandler(ILogger<FirstHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(IMessage message)
        {
            var messageReceived = (FirstMessage)message;
            _logger.LogInformation("FirstHandler message: {FullName}",
                messageReceived.Teste);
            return Task.CompletedTask;
        }

        public static Type MessageType { get; } = typeof(FirstMessage);
    }
}
