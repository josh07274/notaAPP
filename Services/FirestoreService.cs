using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore.V1;
using Microsoft.Maui.Storage;
using System.IO;
using BlocDeNotas.Services;

namespace BlocDeNotas.Services
{
    public class FirestoreService
    {
        private FirestoreDb _firestoreDb;

        public FirestoreService()
        {
            InitializeFirestore();
        }

        private void InitializeFirestore()
        {
            // 🔥 Guardar credenciales en la carpeta de datos al iniciar la app
            FirebaseCredentialManager.SaveFirebaseCredentials();

            string jsonCredentials = FirebaseCredentialManager.LoadFirebaseCredentials();

            if (string.IsNullOrWhiteSpace(jsonCredentials))
            {
                Console.WriteLine("Error: No se pudieron cargar las credenciales.");
                return;
            }

            GoogleCredential credential = GoogleCredential.FromJson(jsonCredentials);
            FirestoreClientBuilder builder = new FirestoreClientBuilder
            {
                Credential = credential
            };
            FirestoreClient firestoreClient = builder.Build();

            _firestoreDb = FirestoreDb.Create("base-de-datos-usuarios-c9142", firestoreClient);
        }

        public async Task<string> AddUserAsync(string username, string password)
        {
            if (_firestoreDb == null)
            {
                Console.WriteLine("Error: Firestore aún no está inicializado.");
                return string.Empty;
            }

            // 🔥 Generar un ID único para el usuario
            string userId = Guid.NewGuid().ToString();

            CollectionReference usersRef = _firestoreDb.Collection("usuarios");
            DocumentReference newUserRef = usersRef.Document(userId); // Guardar con UserId

            Dictionary<string, object> userData = new()
    {
        { "UserId", userId }, // 🔥 Cambiar "Id" a "UserId" para coincidir con la base de datos
        { "Username", username }, // 🔥 Usar "Username" en lugar de "User" para mantener consistencia
        { "Password", password } // ⚠️ Se recomienda usar hashing en lugar de texto plano
    };

            await newUserRef.SetAsync(userData);
            Console.WriteLine($"Usuario {username} agregado correctamente con UserId: {userId}");

            return userId; // 🔥 Retornar el UserId correcto
        }

        public async Task<AppUser?> GetUserByUsernameAsync(string username)
        {
            if (_firestoreDb == null)
            {
                Console.WriteLine("Error: Firestore aún no está inicializado.");
                return null;
            }

            CollectionReference usersRef = _firestoreDb.Collection("usuarios");
            Query query = usersRef.WhereEqualTo("Username", username); // 🔥 Buscar por "Username"
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Documents.Count > 0)
            {
                return snapshot.Documents[0].ConvertTo<AppUser>(); // ✅ Retornar usuario encontrado
            }

            return null;
        }
        public async Task AddNoteAsync(string userId, string title, string content)
        {
            if (_firestoreDb == null)
            {
                Console.WriteLine("Error: Firestore aún no está inicializado.");
                return;
            }

            string noteId = Guid.NewGuid().ToString(); // 🔥 Generar un ID único para la nota

            CollectionReference notasRef = _firestoreDb.Collection("usuarios").Document(userId).Collection("Notas");
            DocumentReference newNoteRef = notasRef.Document(noteId);

            Dictionary<string, object> noteData = new()
    {
        { "Id", noteId }, // 🔥 Guardar siempre el ID único de la nota
        { "Title", title },
        { "Content", content },
        { "Date", DateTime.UtcNow.ToString("dd-MM-yyyy") }
    };

            await newNoteRef.SetAsync(noteData);
            Console.WriteLine($"Nota guardada correctamente con ID: {noteId} para el usuario {userId}");
        }
        public async Task<List<Dictionary<string, object>>> GetUserNotesAsync(string userId)
        {
            if (_firestoreDb == null)
            {
                Console.WriteLine("Error: Firestore aún no está inicializado.");
                return new List<Dictionary<string, object>>();
            }

            CollectionReference notasRef = _firestoreDb.Collection("usuarios").Document(userId).Collection("Notas");
            QuerySnapshot snapshot = await notasRef.GetSnapshotAsync();

            List<Dictionary<string, object>> notes = new();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                notes.Add(doc.ToDictionary());
            }

            return notes;
        }

        public async Task UpdateNoteAsync(string userId, string noteId, string title, string content)
        {
            if (_firestoreDb == null)
            {
                Console.WriteLine("Error: Firestore aún no está inicializado.");
                return;
            }

            DocumentReference noteRef = _firestoreDb.Collection("usuarios").Document(userId).Collection("Notas").Document(noteId);

            Dictionary<string, object> updatedData = new()
    {
        { "Id", noteId }, // 🔥 Mantener el ID de la nota existente
        { "Title", title },
        { "Content", content },
        { "Date", DateTime.UtcNow.ToString("dd-MM-yyyy") }
    };

            await noteRef.SetAsync(updatedData, SetOptions.MergeAll); // 🔥 Merge para actualizar solo los campos modificados
            Console.WriteLine($"Nota {noteId} actualizada correctamente.");
        }
        // Método para eliminar una nota
        public async Task DeleteNoteAsync(string userId, string noteId)
        {
            if (_firestoreDb == null)
            {
                Console.WriteLine("Error: Firestore aún no está inicializado.");
                return;
            }

            DocumentReference noteRef = _firestoreDb.Collection("usuarios").Document(userId).Collection("Notas").Document(noteId);
            await noteRef.DeleteAsync();

            Console.WriteLine($"Nota {noteId} eliminada correctamente para el usuario {userId}.");
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            try
            {
                CollectionReference usersRef = _firestoreDb.Collection("usuarios"); // 📌 Asegurar que la colección se llama "usuarios"
                Query query = usersRef.WhereEqualTo("Username", username); // 🔥 Corregir a "Username" con mayúscula
                QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                return querySnapshot.Documents.Count > 0; // ✅ Si hay registros, el usuario ya existe
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al verificar usuario existente: {ex.Message}");
                return false; // ⚠ En caso de error, asumimos que no existe
            }
        }


    }
}