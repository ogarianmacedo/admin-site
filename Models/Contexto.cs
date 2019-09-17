using Microsoft.EntityFrameworkCore;
using ProjetoAdminSite.Mapeamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdminSite.Models
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> opcoes) : base(opcoes)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<InscricaoExterna> InscricoesExterna { get; set; }

        //Classes Mapeadas
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioMap());
            modelBuilder.ApplyConfiguration(new BlogMap());
            modelBuilder.ApplyConfiguration(new InscricaoExternaMap());
        }
    }
}
