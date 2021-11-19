using Icebear.Exceptions.Core.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Icebear.Exceptions.Core.Models
{
    public class ErrorDbContext: DbContext
    {
        public ErrorDbContext()
        {
            
        }
        public ErrorDbContext(DbContextOptions options): base(options) { }

        public DbSet<LogEntity> Errors { get; set; }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=ExceptionBear;User Id=sa;Password=Welcome1!");
            base.OnConfiguring(optionsBuilder);
        }*/
    }
}