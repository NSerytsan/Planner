namespace Planner.MessageQueue;
public interface IMessageProducer
{
    public Task SendMessageAsync(string message);
}