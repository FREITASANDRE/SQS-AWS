using Amazon.SQS;
using Amazon.SQS.Model;
using MessageBroker.Sqs.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace MessageBroker.Sqs.Consumer
{
    public class SqsConsumerHostedService<TMessage> : BackgroundService where TMessage : IMessage
    {
        private readonly ILogger<SqsConsumerHostedService<TMessage>> _logger;
        private readonly IAmazonSQS _sqs;
        private readonly MessageDispatcher _dispatcher;
        private readonly string _queueName;
        private readonly List<string> _messageAttributeNames = new() { "All" };

        public SqsConsumerHostedService(ILogger<SqsConsumerHostedService<TMessage>> logger, IAmazonSQS sqs, MessageDispatcher dispatcher, string queueName)
        {
            _logger = logger;
            _sqs = sqs;
            _dispatcher = dispatcher;
            _queueName = queueName;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var listQueuesRequest = new ListQueuesRequest
            {
                QueueNamePrefix = _queueName
            };

            var response = await _sqs.ListQueuesAsync(listQueuesRequest);

            if (response.QueueUrls.Count == 0)
            {
                _logger.LogError($"Queue {_queueName} does not exist");
                return;
            }

            var queueUrl = await _sqs.GetQueueUrlAsync(_queueName, cancellationToken);
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl.QueueUrl,
                MessageAttributeNames = _messageAttributeNames,
                AttributeNames = _messageAttributeNames
            };

            while (!cancellationToken.IsCancellationRequested)
            {
                var messageResponse = await _sqs.ReceiveMessageAsync(receiveRequest, cancellationToken);
                if (messageResponse.HttpStatusCode != HttpStatusCode.OK)
                {
                    continue;
                }

                foreach (var message in messageResponse.Messages)
                {
                    var messageTypeName = message.MessageAttributes
                        .GetValueOrDefault(nameof(IMessage.MessageTypeName))?.StringValue;

                    if (messageTypeName is null)
                    {
                        continue;
                    }

                    if (!_dispatcher.CanHandleMessageType(messageTypeName))
                    {
                        continue;
                    }

                    var messageType = _dispatcher.GetMessageTypeByName(messageTypeName)!;

                    var messageAsType = (IMessage)JsonSerializer.Deserialize(message.Body, messageType)!;

                    await _dispatcher.DispatchAsync(messageAsType);
                    await _sqs.DeleteMessageAsync(queueUrl.QueueUrl, message.ReceiptHandle, cancellationToken);
                }
            }
        }
    }
}
