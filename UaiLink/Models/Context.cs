using Microsoft.EntityFrameworkCore;

namespace UaiLink.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
           : base(options)
        {
        }

        public DbSet<UsuarioDto> Usuarios { get; set; }
        public DbSet<ProjetoDto> Projetos { get; set; }
        public DbSet<ComentarioDto> Comentarios { get; set; }
        public DbSet<AnexoDto> Anexos { get; set; }
        public DbSet<TagDto> tags { get; set; }
    }
}
