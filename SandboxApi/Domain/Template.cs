using SandboxApi.Domain.Common;

namespace SandboxApi.Domain;

public class Template
    : Entity
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Version { get; set; }
}
