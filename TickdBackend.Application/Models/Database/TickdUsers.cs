using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TickdBackend.Application.Models.Database
{
    [Table("TickdUsers")]
    public class TickdUsers
    {
        [Key]
        [Column("AccountId")]
        public int AccountId { get; set; }
        [Column("FirstName")]
        public string? FirstName { get; set; }
        [Column("LastName")]
        public string? LastName { get; set; }

    }
}
