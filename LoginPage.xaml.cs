using System;
using Microsoft.Maui.Controls;
namespace NotepadMaui;
using BlocDeNotas; // Ajusta según tu namespace
using Microsoft.Maui.Storage;
using BlocDeNotas.Services; // 🔥 Importa el namespace correcto

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // 🔥 Verificar si el usuario ya tiene sesión iniciada
        bool isLoggedIn = Preferences.Get("IsUserLoggedIn", false);

        if (isLoggedIn)
        {
            string loggedUserId = Preferences.Get("LoggedUserId", string.Empty); // 🔥 Obtener el UserId
            string loggedUsername = Preferences.Get("Username", string.Empty);  // 🔥 Obtener el Username

            if (!string.IsNullOrWhiteSpace(loggedUserId) && !string.IsNullOrWhiteSpace(loggedUsername))
            {
                Navigation.PushAsync(new UserProfilePage(loggedUsername)); // ✅ Pasar el username al constructor
            }
        }
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

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Por favor, ingrese usuario y contraseña.", "OK");
            return;
        }

        FirestoreService firestoreService = new FirestoreService();
        AppUser? user = await firestoreService.GetUserByUsernameAsync(username); // ✅ Buscar usuario por Username

        if (user is not null && user.Password == password)
        {
            // 🔥 Guardar sesión en `Preferences` con UserId y Username
            Preferences.Set("LoggedUserId", user.UserId);
            Preferences.Set("Username", user.Username);
            Preferences.Set("IsUserLoggedIn", true);

            await DisplayAlert("Éxito", "Inicio de sesión correcto.", "OK");
            await Navigation.PushAsync(new MainPage()); // 🔄 Ir a MainPage directamente
        }
        else
        {
            await DisplayAlert("Error", "Usuario o contraseña incorrectos.", "OK");
        }
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage()); // Ir a la página de registro
    }
}