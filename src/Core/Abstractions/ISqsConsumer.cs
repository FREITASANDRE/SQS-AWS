using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBroker.Sqs.Abstractions
{
    public interface ISqsConsumerService<TMessage> where TMessage : IMessage
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}
