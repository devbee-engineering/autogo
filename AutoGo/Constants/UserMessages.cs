namespace AutoGo.Constants
{
    public class UserMessages
    {
        public const string MobileNoNotFoundMessage = "மொபைல் எண் காணப்படவில்லை. தயவுசெய்து மீண்டும் முயற்சிக்கவும்.";

        public const string EnterPinPromptMessage = "தயவுசெய்து உங்கள் 4 இலக்க ரகசிய பின்நம்பரை உள்ளிடவும்.";

        public const string MobileNumberPromptMessage = "உங்கள் கணக்கு இன்னும் சரிபார்க்கப்படவில்லை. தயவுசெய்து உங்கள் மொபைல் எண்ணை உள்ளிடவும்.";

        public const string WrongPinMessage = "தவறான பின். தயவுசெய்து மீண்டும் முயற்சிக்கவும்.";

        public static string AccountVerificationSuccessMessage(string name) => $"உங்கள் கணக்கு வெற்றிகரமாக சரிபார்க்கப்பட்டது. வரவேற்கிறோம், {name}!";

        public static string WelcomeMessage(string name) => $"இதயம் கனிந்த வரவேற்பு, {name}! ❤️\n\n" +
                                                     $"நாங்கள் இங்கே உங்களின் நாளை சிறப்பாகவும் மகிழ்ச்சியுடனும் மாற்ற உதவ தயாராக இருக்கிறோம். உங்கள் தினம் வெற்றியுடனும் சந்தோஷத்துடனும் நிறைந்ததாக இருக்க வாழ்த்துகிறோம்! 🌟😊";

    }
}
