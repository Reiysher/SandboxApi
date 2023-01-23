using SandboxApi.Domain.Common;

namespace SandboxApi.Domain;

public class Transaction
    : Entity
{
    public Guid Id { get; set; }
    public DateTime? FirstTime { get; set; }
    public DateTime? SecondTime { get; set;}
}
