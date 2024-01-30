using MessageBroker.Sqs.Abstractions;
using System.Text.Json.Serialization;

namespace WebApplication1.Messages
{
    public class OtherMessage : IMessage
    {
        public string Teste { get; set; } = string.Empty;

        [JsonIgnore]
        public string MessageTypeName => nameof(OtherMessage);
    }

}
