using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using yourLogs.Exceptions.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace yourLogs.Exceptions.Db.Ef.Models
{
    [Index("Id", IsUnique = true, Name = "UNQ_LOG_ID")]
    [Table("Logs")]
    public record LogEntity : ILogEntry{
        [Key]
        [Column("Id")]
        public Guid Id { get; internal set; }
        
        [MaxLength(255)]
        public string? Code { get; set; }

        [MaxLength(1024)]
        public string? Tags { get; set; }

        public DateTimeOffset OccurredDate { get; internal set; } = DateTimeOffset.Now;
        
        public LogType LogType { get; set; }

        public string? Source { get; internal set; }
        public string? Text { get; internal set; }
        
        public string? Description { get; internal set; }
        public string? UserContext { get; internal set; }
        public string? SystemContext { get; internal set; }
    }
}