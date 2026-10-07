namespace Airbnb.Contracts.Broker;

public interface IMessageService<TMessage>
{
    public Task<Result<bool>> HandleAsync(TMessage message);
}