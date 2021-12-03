using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using yourLogs.Exceptions.Core.Models;

namespace yourLogs.Exceptions.Db.Ef.Models
{
    [Table("Icebear_Tag")]
    public sealed class TagEntity : ITag
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Tag { get; set; } = "";
    }
}