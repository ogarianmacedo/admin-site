using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetoAdminSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdminSite.Mapeamento
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(u => u.UsuarioId);

            builder.Property(u => u.Nome).IsRequired();

            builder.Property(u => u.Email).IsRequired();
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Senha).IsRequired();

            builder.Property(u => u.Ativo).IsRequired().HasDefaultValue(true);

            builder.HasMany(u => u.Blogs).WithOne(u => u.Usuario).OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Usuarios");
        }
    }
}
