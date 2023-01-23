using Mapster;
using SandboxApi.Domain;
using SandboxApi.Models.Common;

namespace SandboxApi.Models;

public class TicketDto
    : BaseDto<TicketDto, Ticket>
{
	public TicketDto() { }

    public Guid Id { get; set; }
    public string? TicketName { get; set; }
    public List<FieldDto>? Fields { get; set; }

    public override void AddCustomMappings()
    {
        SetCustomMappings()
            .Map(dest => dest.Fields, src => src.Fields);
    }
}
