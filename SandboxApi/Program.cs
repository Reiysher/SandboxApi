using Mapster;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SandboxApi.Converters;
using SandboxApi.Domain.Events;
using SandboxApi.Persistence;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new FieldJsonConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    options.UseNpgsql(builder.Configuration.GetSection("ConnectionStrings:Default").Get<string>(), builder =>
    {
        builder.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
        builder.EnableRetryOnFailure();
        builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
    });
});

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<TransactionsStartedDomainEventHandler>();

    config.UsingInMemory((ctx, cfg) =>
    {
        cfg.ConfigureEndpoints(ctx);
    });
});


builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

// Context
builder.Services.AddScoped<ApplicationDbContext>();

// Mapster
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Database initialize
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
