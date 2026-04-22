using Microsoft.EntityFrameworkCore;

namespace SosDog.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Ocorrencia> Ocorrencias { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Favorito> Favoritos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Evitando problemas de "Multiple Cascade Paths" (Múltiplos caminhos de exclusão em cascata)
            // Se um usuário for apagado, apagamos os comentários, mas deixamos a exclusão da ocorrência manual/desativada para não dar conflito.
            modelBuilder.Entity<Comentario>()
                .HasOne(c => c.Ocorrencia)
                .WithMany(o => o.Comentarios)
                .HasForeignKey(c => c.ID_Ocorrencia)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favorito>()
                .HasOne(f => f.Ocorrencia)
                .WithMany(o => o.FavoritadosPor)
                .HasForeignKey(f => f.ID_Ocorrencia)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}