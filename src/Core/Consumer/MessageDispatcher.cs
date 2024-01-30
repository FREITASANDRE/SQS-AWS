using MessageBroker.Sqs.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MessageBroker.Sqs.Consumer
{
    public class MessageDispatcher
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly Dictionary<string, Type> _messageMappings;

        private readonly Dictionary<string, Func<IServiceProvider, IMessageHandler>> _handlers;

        public MessageDispatcher(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            var messagesMappings = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.DefinedTypes.Where(x => typeof(IMessage).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract));
            var messagesHandlers = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.DefinedTypes.Where(x => typeof(IMessageHandler).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract));
            _messageMappings = messagesMappings.ToDictionary(info => info.Name, info => info.AsType());

            _handlers = messagesHandlers
                .ToDictionary<TypeInfo, string, Func<IServiceProvider, IMessageHandler>>(
                info => ((Type)info.GetProperty(nameof(IMessageHandler.MessageType))!.GetValue(null)!)!.Name,
                info => provider => (IMessageHandler)provider.GetRequiredService(info.AsType()));
        }

        public async Task DispatchAsync<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            using var scope = _scopeFactory.CreateScope();
            var handler = _handlers[message.MessageTypeName](scope.ServiceProvider);
            await handler.HandleAsync(message);
        }

        public bool CanHandleMessageType(string messageTypeName)
        {
            return _handlers.ContainsKey(messageTypeName);
        }

        public Type? GetMessageTypeByName(string messageTypeName)
        {
            return _messageMappings.GetValueOrDefault(messageTypeName);
        }
    }

}
