using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SandboxApi.Domain;
using SandboxApi.Domain.Events;
using SandboxApi.Models;
using SandboxApi.Persistence;
using System.Net.Mime;
using System.Text.Json;

namespace SandboxApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IBus _messageBus;

    public TestController(
        ApplicationDbContext dbContext,
        IBus messageBus)
    {
        _dbContext = dbContext;
        _messageBus = messageBus;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Get()
    {
        var result = await _dbContext.Tickets
            .Include(t => t.Fields)
            .Where(t => t.Fields!.OfType<DateField>().Any(f => f.Value > DateTime.UtcNow.AddMinutes(-1)))
            .FirstOrDefaultAsync();

        var dto = result?.Adapt<TicketDto>();

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var stringField = new StringField
        {
            Name = "String field",
            Description = "String field description",
            Value = $"String value {Random.Shared.Next(0, 500)}"
        };

        var numberField = new NumberField
        {
            Name = "Number field",
            Description = "Number field description",
            Value = Random.Shared.Next(0, 500)
        };

        var dateField = new DateField
        {
            Name = "Date field",
            Description = "Date field description",
            Value = DateTime.UtcNow
        };

        var ticket = new Ticket
        {
            Fields = new List<Field>() { stringField, numberField, dateField }
        };

        await _dbContext.Tickets.AddRangeAsync(ticket);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }
}
