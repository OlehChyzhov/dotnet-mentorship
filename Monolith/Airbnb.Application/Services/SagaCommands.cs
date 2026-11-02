namespace Airbnb.Application.Services;

public record StartTransactionCommand(Guid CommandId);
public record ExecuteSecondCommand(Guid CommandId);
public record ExecuteThirdCommand(Guid CommandId);
public record ExecuteFourthCommand(Guid CommandId);
public record CompleteTransactionCommand(Guid CommandId);