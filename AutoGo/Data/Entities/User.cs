using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGo.Data.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; } // Unique identifier for the user

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; } // User's name, max length 100 characters

        [Required]
        [MaxLength(15)]
        [MinLength(10)]
        public required string MobileNumber { get; set; } // Unique mobile number, 10-15 characters

        [Required]
        [MaxLength(4)]
        public required string Pin { get; set; } // 4-digit PIN for activation

        [Required]
        public bool IsVerified { get; set; } // Indicates if the user is verified

        [Required]
        public UserType UserType { get; set; } // Enum for user type (Admin, Driver, Super Driver)


        [Required]
        [Column("OrganizationIds", TypeName = "integer[]")]
        public required int[] OrganizationIds { get; set; }

        public long? TelegramUserId { get; set; }

        public bool IsOnline { get; set; }  
    }
}
