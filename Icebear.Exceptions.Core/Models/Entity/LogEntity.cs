using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Icebear.Exceptions.Core.Models.Entity
{
    [Index("Id", IsUnique = true, Name = "UNQ_LOG_ID")]
    [Table("Logs")]
    public record LogEntity : IError{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id", TypeName = @"varchar(32)")]
        public string Id { get; internal init; }
        
        [MaxLength(255)]
        public string Code { get; init; }

        public DateTimeOffset OccurredDate { get; internal set; } = DateTimeOffset.Now;
        
        public LogType LogType { get; init; }

        public string Source { get; internal set; }
        public string Text { get; internal set; }
        
        public string Description { get; internal set; }
    }
}