namespace AutoGo.Constants;

public static class AdminMessages
{
    public const string BookingCreateMobileNoPromptMessage = "புதிய புக்கிங் உருவாக்க, தயவுசெய்து வாடிக்கையாளர் மொபைல் எண்ணை உள்ளிடவும்.";
    public const string BookingCreateVoicePromptMessage = "புதிய புக்கிங் உருவாக்க, தயவுசெய்து வாடிக்கையாளர் தொடர்பான தகவலுடன் ஒரு குரல் குறிப்பை பதிவேற்றவும்.";
    public static string BookingCreatedMessage(long bookingId) => $"உங்கள் புக்கிங் உருவாக்கப்பட்டுள்ளது! புக்கிங் ஐடி: {bookingId}\n\n" +
                                                           $"தயவுசெய்து, டிரைவர்களுக்கு ஒதுக்கப்படும் வரை காத்திருக்கவும். நன்றி!";

    public static string BookingAssignedToDriverMessage(long bookingId, string name, string mobileNo) =>
            $"📢 புக்கிங் {bookingId} ✅ டிரைவர் {name} அவர்களுக்கு ஒதுக்கப்பட்டுள்ளது.\n📞 மொபைல் எண்: {mobileNo}";

    public static string BookingCompletedMessage(long bookingId, string name, string mobileNo) =>
    $"📢 புக்கிங் {bookingId}, வெற்றிகரமாக முடிக்கப்பட்டது {name}, {mobileNo} அவர்களால்.";
}
