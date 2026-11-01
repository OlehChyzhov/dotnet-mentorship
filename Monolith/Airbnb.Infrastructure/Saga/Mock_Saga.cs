using MassTransit;
using Microsoft.Extensions.Logging;

namespace Airbnb.Infrastructure.Saga;

public record StartTransactionCommand(Guid CommandId);
public record ExecuteSecondCommand(Guid CommandId);
public record ExecuteThirdCommand(Guid CommandId);
public record ExecuteFourthCommand(Guid CommandId);
public record CompleteTransactionCommand(Guid CommandId);

public record StartTransactionCommandExecutedEvent(Guid CommandId);
public record SecondCommandExecutedEvent(Guid CommandId);
public record ThirdCommandExecutedEvent(Guid CommandId);
public record FourthCommandExecutedEvent(Guid CommandId);

public class StartTransactionCommandHandler(ILogger<StartTransactionCommandHandler> logger) : IConsumer<StartTransactionCommand>
{
    public async Task Consume(ConsumeContext<StartTransactionCommand> context)
    {
        logger.LogInformation("Entered First Command Handler {CommandId}", context.Message.CommandId);
        
        // Do logic
        
        await context.Publish(new StartTransactionCommandExecutedEvent(context.Message.CommandId));
    }
}

public class SecondCommandHandler(ILogger<SecondCommandHandler> logger) : IConsumer<ExecuteSecondCommand>
{
    public async Task Consume(ConsumeContext<ExecuteSecondCommand> context)
    {
        logger.LogInformation("Entered Second Command Handler {CommandId}", context.Message.CommandId);
        
        // Do logic
        
        await context.Publish(new SecondCommandExecutedEvent(context.Message.CommandId));
    }
}

public class ThirdCommandHandler(ILogger<ThirdCommandHandler> logger) : IConsumer<ExecuteThirdCommand>
{
    public async Task Consume(ConsumeContext<ExecuteThirdCommand> context)
    {
        logger.LogInformation("Entered Third Command Handler {CommandId}", context.Message.CommandId);
        
        // Do logic
        
        await context.Publish(new ThirdCommandExecutedEvent(context.Message.CommandId));
    }
}

public class FourthCommandHandler(ILogger<FourthCommandHandler> logger) : IConsumer<ExecuteFourthCommand>
{
    public async Task Consume(ConsumeContext<ExecuteFourthCommand> context)
    {
        logger.LogInformation("Entered Fourth Command Handler {CommandId}", context.Message.CommandId);
        
        // Do logic
        
        await context.Publish(new FourthCommandExecutedEvent(context.Message.CommandId));
    }
}

public class TransactionCompletedEventHandler(ILogger<TransactionCompletedEventHandler> logger) : IConsumer<CompleteTransactionCommand>
{
    public async Task Consume(ConsumeContext<CompleteTransactionCommand> context)
    {
        logger.LogInformation("Entered Transaction Completed Event Handler {CommandId}", context.Message.CommandId);
        
        // Do logic
    }
}


public class MockSagaStateMachineInstance : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    
    public Guid SomeId { get; set; }
    public string RandomValue { get; set; }
}

public class MockSagaStateMachine : MassTransitStateMachine<MockSagaStateMachineInstance>
{
    public State SecondState { get; set; }
    public State ThirdState { get; set; }
    public State FourthState { get; set; }
    
    public Event<StartTransactionCommandExecutedEvent> StartTransactionCommandExecuted { get; set; }
    public Event<SecondCommandExecutedEvent> SecondCommandExecuted { get; set; }
    public Event<ThirdCommandExecutedEvent> ThirdCommandExecuted { get; set; }
    public Event<FourthCommandExecutedEvent> FourthCommandExecuted { get; set; }
    
    public MockSagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => StartTransactionCommandExecuted, e => e.CorrelateById(m => m.Message.CommandId));
        Event(() => SecondCommandExecuted, e => e.CorrelateById(m => m.Message.CommandId));
        Event(() => ThirdCommandExecuted, e => e.CorrelateById(m => m.Message.CommandId));
        Event(() => FourthCommandExecuted, e => e.CorrelateById(m => m.Message.CommandId));

        Initially(
            When(StartTransactionCommandExecuted)
                .Then(context =>
                {
                    context.Saga.SomeId = context.Message.CommandId;
                    context.Saga.RandomValue = "Abracadabra Initial";
                })
                .TransitionTo(SecondState)
                .Publish(context => new ExecuteSecondCommand(context.Message.CommandId)));
        
        During(SecondState,
            When(SecondCommandExecuted)
                .Then(context =>
                {
                    context.Saga.RandomValue = "Abracadabra First State";
                })
                .TransitionTo(ThirdState)
                .Publish(context => new ExecuteThirdCommand(context.Message.CommandId)));
        
        During(ThirdState,
            When(ThirdCommandExecuted)
                .Then(context =>
                {
                    context.Saga.RandomValue = "Abracadabra Second State";
                })
                .TransitionTo(FourthState)
                .Publish(context => new ExecuteFourthCommand(context.Message.CommandId)));
        
        During(FourthState,
            When(FourthCommandExecuted)
                .Then(context =>
                {
                    context.Saga.RandomValue = "Abracadabra Third State";
                })
                .Publish(context => new CompleteTransactionCommand(context.Message.CommandId))
                .Finalize());
    }
}