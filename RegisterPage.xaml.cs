using Microsoft.Maui.Controls;
using BlocDeNotas.Services;
using Google.Cloud.Firestore;
namespace NotepadMaui;


public partial class RegisterPage : ContentPage
{
    private readonly FirestoreService _firestoreService = new FirestoreService();

    public RegisterPage()
    {
        InitializeComponent();
    }
    protected override bool OnBackButtonPressed()
    {
        bool isLoggedIn = Preferences.Get("IsUserLoggedIn", false);

        if (isLoggedIn)
        {
            Application.Current.MainPage = new NavigationPage(new MainPage()); // 🔄 Redirigir siempre a MainPage
            return true; // 🔥 Bloquear la navegación normal
        }

        return base.OnBackButtonPressed(); // 🔄 Permitir navegación normal si no hay sesión iniciada
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Por favor, completa todos los campos.", "OK");
            return;
        }

        FirestoreService firestoreService = new FirestoreService();

        // 🔍 Verificar si el usuario ya existe en Firestore
        bool userExists = await firestoreService.UserExistsAsync(username);
        if (userExists)
        {
            await DisplayAlert("Error", "Este nombre de usuario ya está en uso. Elige otro.", "OK");
            return;
        }

        // ✅ Registrar usuario si no existe
        string userId = await firestoreService.AddUserAsync(username, password);

        if (!string.IsNullOrWhiteSpace(userId))
        {
            Preferences.Set("LoggedUserId", userId); // 🔥 Guardar la sesión con el ID
            await DisplayAlert("Éxito", "Usuario registrado correctamente.", "OK");
            await Navigation.PushAsync(new LoginPage()); // Redirigir a LoginPage
        }
        else
        {
            await DisplayAlert("Error", "Hubo un problema al registrar el usuario.", "OK");
        }
    }

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }
}