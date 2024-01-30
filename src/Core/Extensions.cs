using Amazon.SQS;
using MessageBroker.Sqs.Abstractions;
using MessageBroker.Sqs.Consumer;
using MessageBroker.Sqs.Publisher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessageBroker.Sqs
{
    public static class Extensions
    {
        public static void AddSqsConsumerService<TMessage, THandler>(this IServiceCollection services, string queueName)
                                                                     where TMessage : IMessage
                                                                     where THandler : class, IMessageHandler
        {
            services.AddTransient<THandler>();
            services.AddTransient<ISqsConsumerService<TMessage>, SqsConsumerService<TMessage>>(provider =>
                new SqsConsumerService<TMessage>(provider.GetRequiredService<ILogger<SqsConsumerService<TMessage>>>(), provider.GetRequiredService<IAmazonSQS>(), provider.GetRequiredService<MessageDispatcher>(), queueName));

            services.AddHostedService(provider => new SqsConsumerHostedService<TMessage>(provider.GetRequiredService<ISqsConsumerService<TMessage>>()));
        }

        public static void AddSqsPublisher(this IServiceCollection services)
        {
            services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(Amazon.RegionEndpoint.USEast1));
            services.AddScoped<ISqsPublisher, SqsPublisher>();
        }

        public static void AddSqsConsumer(this IServiceCollection services)
        {
            services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(Amazon.RegionEndpoint.USEast1));
            services.AddSingleton<MessageDispatcher>();
        }
    }
}
