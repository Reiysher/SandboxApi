using SandboxApi.Domain.Common;

namespace SandboxApi.Domain;

public class Ticket
    : Entity
{
    public Guid Id { get; set; }

    public string? TicketName { get; set; }

    public ICollection<Field>? Fields { get; set; }
}
