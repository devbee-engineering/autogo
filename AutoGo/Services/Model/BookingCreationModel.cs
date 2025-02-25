namespace AutoGo.Services.Model
{
    public class BookingCreationModel
    {
        public int createdById { get; set; }

        public required string MobileNumber { get; set; }

        public string? VoiceFileId { get; set; }

    }
}
