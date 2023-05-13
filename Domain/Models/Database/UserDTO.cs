using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Domain.Models.Database
{
    [Table("users")]
    [Index(nameof(Email), IsUnique = true)]
    public class UserDTO
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("email")]
        [EmailAddress]
        public string Email { get; set; }
    }
}