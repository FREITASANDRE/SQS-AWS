namespace MessageBroker.Sqs.Abstractions
{
    public interface IMessage
    {
        public string MessageTypeName { get; }
    }

}
