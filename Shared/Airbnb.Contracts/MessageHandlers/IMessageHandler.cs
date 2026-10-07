namespace Airbnb.Contracts.MessageHandlers;

public interface IMessageHandler<TMessage>
{
    public Task HandleAsync(TMessage message);
}