using SandboxApi.Domain.Common;

namespace SandboxApi.Domain;

public class Field
    : Entity
{
    public Guid Id { get; set; }

    public Guid? TemplateId { get; set; }

    public Template? Template { get; set; }

    public Guid? TicketId { get; set; }

    public Ticket? Ticket { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }
}
