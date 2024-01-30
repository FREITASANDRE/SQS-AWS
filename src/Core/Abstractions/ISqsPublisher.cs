namespace MessageBroker.Sqs.Abstractions
{
    public interface ISqsPublisher
    {
        Task PublishAsync<TMessage>(string queueName, TMessage message)
            where TMessage : IMessage;
    }
}
