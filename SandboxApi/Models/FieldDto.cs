using SandboxApi.Domain;
using SandboxApi.Models.Common;

namespace SandboxApi.Models;

public class FieldDto
    : BaseDto<FieldDto, Field>
{
    public FieldDto() { }

    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public override void AddCustomMappings()
    {
        SetCustomMappings()
            .Include<StringFieldDto, StringField>()
            .Include<NumberFieldDto, NumberField>()
            .Include<DateFieldDto, DateField>();

        SetCustomMappingsInverse()
            .Include<StringField, StringFieldDto>()
            .Include<NumberField, NumberFieldDto>()
            .Include<DateField, DateFieldDto>();
    }
}
