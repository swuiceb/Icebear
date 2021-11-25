using Icebear.Exceptions.Db.Ef.Models;
using Microsoft.EntityFrameworkCore;

namespace Icebear.Exceptions.Db.Ef
{
    public class ErrorDbContext : DbContext
    {
        public ErrorDbContext()
        {
        }

        public ErrorDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<LogEntity> Logs { get; set; }
        
        public DbSet<TagEntity> Tags { get; set; }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=ExceptionBear;User Id=sa;Password=Welcome1!");
        base.OnConfiguring(optionsBuilder);
    }*/
    }
}