using Google.Cloud.Firestore;

namespace BlocDeNotas 
{
    [FirestoreData]
    public class AppUser
    {
        [FirestoreProperty]
        public string Username { get; set; } = string.Empty; 

        [FirestoreProperty]
        public string Password { get; set; } = string.Empty;

        [FirestoreProperty]
        public string UserId { get; set; } = Guid.NewGuid().ToString();
    }
}