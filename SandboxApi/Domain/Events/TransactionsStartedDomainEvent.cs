using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SandboxApi.Persistence;

namespace SandboxApi.Domain.Events;

public class TransactionsStartedDomainEvent
{
    public Guid? Id { get; init; }

    public TransactionsStartedDomainEvent() { }

    public TransactionsStartedDomainEvent(Guid? formId) =>
        Id = formId;
}

public class TransactionsStartedDomainEventHandler
    : IConsumer<TransactionsStartedDomainEvent>
{
    private readonly ApplicationDbContext _dbContext;

    public TransactionsStartedDomainEventHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<TransactionsStartedDomainEvent> context)
    {
        var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == context.Message.Id);

        await Task.Delay(Random.Shared.Next(0,5000));

        if (transaction != null)
        {
            transaction.SecondTime = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(context.CancellationToken);
        }
    }

    //public async Task Handle(
    //    TransactionsStartedDomainEvent notification, 
    //    CancellationToken cancellationToken)
    //{
    //    var scope = _serviceProvider.CreateScope();
    //    var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //    var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == notification.Id);

    //    await Task.Delay(10000);

    //    if (transaction != null)
    //    {
    //        transaction.SecondTime = DateTime.UtcNow;
    //        await _dbContext.SaveChangesAsync(cancellationToken);
    //    }
    //}
}
