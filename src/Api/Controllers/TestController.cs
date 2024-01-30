using MessageBroker.Sqs.Abstractions;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Messages;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TesteController : ControllerBase
    {
        private readonly ISqsPublisher _sqsPublisher;

        public TesteController(ISqsPublisher sqsPublisher)
        {
            _sqsPublisher = sqsPublisher;
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {

            var testeOther = new OtherMessage
            {
                Teste = "teste othermessage"
            };

            await _sqsPublisher.PublishAsync("otherqueue", testeOther);

            var testeFirst = new FirstMessage
            {
                Teste = "teste firstmessage"
            };

            await _sqsPublisher.PublishAsync("firstqueue", testeFirst);

            return Ok();
        }
    }
}
