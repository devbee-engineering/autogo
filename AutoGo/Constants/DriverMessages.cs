using AutoGo.Data.Entities;

namespace AutoGo.Constants
{
    public static class DriverMessages
    {
        public static string NewBookingMessage(long bookingId) => $"📢 *புதிய புக்கிங் வந்துள்ளது!*\n\n" +
                     $"🆔 *புக்கிங் ஐடி*: {bookingId}\n" +
                     $"🚖 தயவுசெய்து இந்த புக்கிங்கை ஏற்க தயார் நிலையில் இருங்கள்.";

        public const string Accept = "✅ ஏற்கவும்";
        public const string Decline = "❌ நிராகரிக்கவும்";

        public static string BookingAssignmentMessage(long bookingId, string mobileNo) => $"📢 *புக்கிங் ஒதுக்கப்பட்டுள்ளது!*\n\n" +
                 $"🆔 *புக்கிங் ஐடி*: {bookingId}\n" +
                 $"📞 *வாடிக்கையாளர் மொபைல்*: {mobileNo}\n\n" +
                 $"✅ புக்கிங் உங்களுக்கு ஒதுக்கப்பட்டுள்ளது. தயவுசெய்து வாடிக்கையாளரை தொடர்பு கொண்டு புக்கிங்கை நிறைவேற்றவும்.";

        public static string BookingAssignmentFailureMessage(long bookingId) => $"❌ *புக்கிங் ஒதுக்கப்படவில்லை!*\n\n" +
                         $"🆔 *புக்கிங் ஐடி*: {bookingId}\n" +
                         $"மன்னிக்கவும், புக்கிங் உங்களுக்கு ஒதுக்கப்படவில்லை. இது மற்றொரு டிரைவரால் ஏற்கப்பட்டிருக்கலாம் அல்லது ரத்து செய்யப்பட்டிருக்கலாம். உங்கள் புரிதலுக்கு நன்றி.";

        public const string Start = "🚗 தொடங்கவும்";
        public const string Complete = "✔️ முடிக்கவும்";

        public static string BookingAcknowledgeAcceptMessage(string bookingId) => $"✅ நீங்கள் புக்கிங் ஐடியை ஏற்றுக்கொண்டீர்கள்: {bookingId}\n\n தயவுசெய்து பயணம் ஒதுக்கப்படும் வரை காத்திருக்கவும்.";

        public static string BookingAcknowledgeDeclinedMessage(string bookingId) => $"❌ நீங்கள் புக்கிங் ஐடியை நிராகரித்தீர்கள்: {bookingId}";


        public const string GoOnlineMessage = "✅ நீங்கள் இப்போது ஆன்லைனில் இருக்கிறீர்கள்! 😊\n\n இன்று உங்களுக்கு ஒரு சிறந்த நாளாக இருக்க வாழ்த்துகிறோம். 🌟";

        public const string GoOfflineMessage = "❌ நீங்கள் இப்போது ஆஃப்லைனில் இருக்கிறீர்கள்! 😊\n\n தயார் ஆனவுடன் மீண்டும் ஆன்லைனில் செல்ல மறக்காதீர்கள். 🙏 நன்றி! 🙌";

        public static string PromptForScreenShotMessage(string bookingId) =>
    $"தயவுசெய்து உங்கள் மீட்டர் ஆப்ளிகேஷனின் ஸ்கிரீன்ஷாட்டை இணைக்கவும், புக்கிங் {bookingId} ஐ முடிக்க.";
    }
}
