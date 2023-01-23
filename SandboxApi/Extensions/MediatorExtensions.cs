using MediatR;
using SandboxApi.Domain.Common;
using SandboxApi.Persistence;
using System.Collections.Concurrent;

namespace SandboxApi.Extensions;

internal static class MediatorExtensions
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, ApplicationDbContext context)
    {
        var domainEntities = context.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}