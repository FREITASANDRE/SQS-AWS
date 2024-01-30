using MessageBroker.Sqs;
using Handlers;
using WebApplication1.Messages;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSqsConsumer();
        builder.Services.AddSqsPublisher();
        builder.Services.AddSqsConsumerService<OtherMessage, OtherHandlers>("otherqueue");
        builder.Services.AddSqsConsumerService<FirstMessage, FirstHandler>("firstqueue");

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}