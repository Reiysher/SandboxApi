using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SandboxApi.Domain;
using SandboxApi.Extensions;

namespace SandboxApi.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Field> Fields => Set<Field>();
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    private readonly IMediator _mediator;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator)
        : base(options)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>()
            .Property(e => e.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Transaction>().ToTable("transactions");
        modelBuilder.Entity<Field>().ToTable("fields");
        modelBuilder.Entity<Template>().ToTable("templates");

        modelBuilder.Entity<StringField>()
            .ToTable("fields_strings")
            .Property(f => f.Value)
            .HasColumnName("string_value");

        modelBuilder.Entity<NumberField>()
            .ToTable("fields_numbers")
            .Property(f => f.Value)
            .HasColumnName("number_value");

        modelBuilder.Entity<DateField>()
            .ToTable("fields_dates")
            .Property(f => f.Value)
            .HasColumnName("date_value");

        modelBuilder.Entity<Ticket>()
            .ToTable("tickets")
            .HasMany(t => t.Fields)
            .WithOne(f => f.Ticket)
            .HasForeignKey(f => f.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        // Есть 2 варианта:
        // 1 - Публиковать события до сохранения(одна транзакция в бд).
        // 2 - После сохранения(на каждое событие своя транзакция).
        //await _mediator.DispatchDomainEventsAsync(this);

        await base.SaveChangesAsync(cancellationToken);

        await _mediator.DispatchDomainEventsAsync(this);

        return true;
    }
}
