using System.ComponentModel.DataAnnotations;

namespace AutoGo.Data.Entities;

public class Organization
{
    [Key]
    [Required]
    public int Id { get; set; } // Unique identifier for the organization

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } // Organization name, max length 100 characters

}
