using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetoAdminSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdminSite.Mapeamento
{
    public class InscricaoExternaMap : IEntityTypeConfiguration<InscricaoExterna>
    {
        public void Configure(EntityTypeBuilder<InscricaoExterna> builder)
        {
            builder.HasKey(ie => ie.InscricaoExternaId);

            builder.Property(ie => ie.Nome).IsRequired();

            builder.Property(ie => ie.Email).IsRequired();

            builder.Property(ie => ie.Telefone).IsRequired();

            builder.Property(ie => ie.Mensagem).IsRequired();

            builder.Property(b => b.DtInscricao).IsRequired().HasDefaultValue(DateTime.Now);

            builder.ToTable("InscricoesExterna");
        }
    }
}
