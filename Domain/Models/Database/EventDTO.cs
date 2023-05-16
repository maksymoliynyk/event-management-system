using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Database
{
    [Table("events")]
    public class EventDTO
    {
        [Column("id")]
        [Key]
        public Guid Id { get; set; }
        [Column("title")]
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Column("description")]
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        [Column("date")]
        [Required]
        public DateTimeOffset Date { get; set; }
        [Column("duration")]
        [Required]
        public TimeSpan Duration { get; set; }
        [Column("location")]
        [Required]
        [MaxLength(100)]
        public string Location { get; set; }
        [Column("owner_id")]
        [Required]
        public Guid OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public virtual UserDTO Owner { get; set; }
        [Column("status")]
        public int Status { get; set; }
    }
}