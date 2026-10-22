using Microsoft.EntityFrameworkCore;
using Seguradora.Domain.Apolices;
using Seguradora.Domain.Sinistros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Infrastructure.Persistence.Configurations
{
    internal sealed class SinistroConfiguration : IEntityTypeConfiguration<Sinistro>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Sinistro> builder)
        {
            builder.ToTable("Sinistros");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedNever();
            builder.Property(s => s.Descricao).IsRequired().HasMaxLength(1000);
            builder.Property(s => s.ValorEstimado).IsRequired().HasPrecision(18, 2);
            builder.Property(s => s.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
            builder.Property(s => s.MotivoNegativa).HasMaxLength(1000);

            builder.HasOne<Apolice>()
                .WithMany()
                .HasForeignKey(s => s.ApoliceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
