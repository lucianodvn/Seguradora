using MassTransit;
using Microsoft.EntityFrameworkCore;
using Seguradora.Application.Abstractions;
using Seguradora.Domain.Apolices;
using Seguradora.Domain.Sinistros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Infrastructure.Persistence
{
    internal sealed class SeguradoraDbContext(DbContextOptions<SeguradoraDbContext> options) : DbContext(options), IUnitOfWork
    {
        public DbSet<Apolice> Apolices => Set<Apolice>();
        public DbSet<Sinistro> Sinistros => Set<Sinistro>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeguradoraDbContext).Assembly);
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }

        public Task<int> SalvarAsync(CancellationToken cancellationToken) => SaveChangesAsync(cancellationToken);

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) =>
            configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
    }
}
