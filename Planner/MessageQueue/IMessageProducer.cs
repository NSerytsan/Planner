namespace Planner.MessageQueue;
public interface IMessageProducer
{
    void SendMessage(string message);
}