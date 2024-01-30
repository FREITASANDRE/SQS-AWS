using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBroker.Sqs.Abstractions
{
    public interface IMessageHandler
    {
        public Task HandleAsync(IMessage message);

        public static Type MessageType { get; }
    }
}
