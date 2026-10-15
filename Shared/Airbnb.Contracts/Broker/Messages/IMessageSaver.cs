namespace Airbnb.Contracts.Broker.Messages;

public interface IMessageSaver<TMessage>
{
    public Task<Result<bool>> HandleAsync(TMessage message);
}