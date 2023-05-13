using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Domain.Models.Database
{
    [Table("rsvps")]
    [Index(nameof(EventId), nameof(UserId), IsUnique = true)]
    public class RSPVDTO
    {
        [Column("id")]
        [Key]
        public Guid Id { get; set; }
        [Column("event_id")]
        public Guid EventId { get; set; }
        [ForeignKey(nameof(EventId))]
        public virtual EventDTO Event { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual UserDTO User { get; set; }
        [Column("status")]
        public int Status { get; set; }
    }
}