using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Komentarze)
                .WithOne(k => k.Post)
                .HasForeignKey(k => k.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Komentarz> Komentarze { get; set; }
    }
}
