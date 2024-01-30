using MessageBroker.Sqs.Abstractions;
using WebApplication1.Messages;

namespace Handlers
{
    public class OtherHandlers : IMessageHandler
    {
        private readonly ILogger<OtherHandlers> _logger;

        public OtherHandlers(ILogger<OtherHandlers> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(IMessage message)
        {
            var messageReceived = (OtherMessage)message;
            _logger.LogInformation("OtherHandler message: {FullName}",
                messageReceived.Teste);
            return Task.CompletedTask;
        }

        public static Type MessageType { get; } = typeof(OtherMessage);
    }
}
