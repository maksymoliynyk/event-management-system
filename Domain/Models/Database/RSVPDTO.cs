using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Domain.Models.Database
{
    [Table("rsvps")]
    [Index(nameof(EventId), nameof(UserId), IsUnique = true)]
    public class RSVPDTO
    {
        [Column("id")]
        [Key]
        public string Id { get; set; }
        [Column("event_id")]
        public string EventId { get; set; }
        [ForeignKey(nameof(EventId))]
        public virtual EventDTO Event { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual UserDTO User { get; set; }
        [Column("status")]
        public int Status { get; set; }
    }
}