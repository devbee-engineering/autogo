using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGo.Data.Entities;

public class Booking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public int CreatedById { get; set; }

    [Required]
    public DateTime CreatedTime { get; set; }

    public int? AssignedToId { get; set; }

    public DateTime? AssignedToTime { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? CompletedTime { get; set; }


    [Required]
    public int OrganizationId { get; set; }

    [Required]
    public BookingStatus Status { get; set; }

    [MaxLength(1000)]
    public string? VoiceFileId { get; set; }

    [MaxLength(13)]
    public string MobileNumber { get; set;}
    [MaxLength(1000)]
    public string? AttachmentId { get; set; }

}
