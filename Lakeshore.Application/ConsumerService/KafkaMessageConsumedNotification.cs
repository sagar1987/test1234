using MediatR;

namespace Lakeshore.Application.ConsumerService
{
    public record KafkaMessageConsumedNotification(string Message) : INotification;
}
