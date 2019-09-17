using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetoAdminSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdminSite.Mapeamento
{
    public class BlogMap : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.HasKey(b => b.BlogId);

            builder.Property(b => b.Titulo).IsRequired();
            builder.HasIndex(b => b.Titulo).IsUnique();

            builder.Property(b => b.Texto).IsRequired();

            builder.Property(b => b.Imagem).IsRequired();

            builder.Property(b => b.DtPublicacao).IsRequired().HasDefaultValue(DateTime.Now);

            builder.Property(b => b.Ativo).IsRequired().HasDefaultValue(true);

            builder.HasOne(b => b.Usuario).WithMany(b => b.Blogs).HasForeignKey(b => b.UsuarioId);

            builder.ToTable("Blogs");
        }
    }
}
