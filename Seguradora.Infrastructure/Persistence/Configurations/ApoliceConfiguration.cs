using Microsoft.EntityFrameworkCore;
using Seguradora.Domain.Apolices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Infrastructure.Persistence.Configurations
{
    internal sealed class ApoliceConfiguration : IEntityTypeConfiguration<Apolice>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Apolice> builder)
        {
            builder.ToTable("Apolices");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedNever();
            builder.Property(a => a.Segurado).IsRequired().HasMaxLength(200);
            builder.Property(a => a.ValorCobertura).IsRequired().HasPrecision(18, 2);
            builder.Property(a => a.Status).IsRequired().HasConversion<string>().HasMaxLength(20).IsRequired();
        }
    }
}
