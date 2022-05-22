using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ASPNetCoreAPI.Entities
{
    [Table("users")]
    public class UserEntity
    {
        [Column("id")]
        public long Id { get; set; }

        [Column("first_name"), Required]
        public string FirstName { get; set; }

        [Column("last_name"), Required]
        public string LastName { get; set; }

        [Column("username"), Required]
        public string Username { get; set; }

        [Column("password_hash"), Required, JsonIgnore]
        public string PasswordHash { get; set; }

        [Column("icon")]
        public string? Icon { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("active")]
        public bool Active { get; set; } = true;

        [Column("create_at")]
        public DateTime? CreateAt { get; set; } = DateTime.UtcNow;
    }
}