using System;
using System.IO;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using BlocDeNotas.Services;
using System.Diagnostics;
using Microsoft.Maui.Storage;
using SkiaSharp;

namespace NotepadMaui
{
    public partial class UserProfilePage : ContentPage
    {
        private readonly FirestoreService _firestoreService = new FirestoreService();
        private readonly string _userId;
        private readonly string imagePath;

        public UserProfilePage(string username)
        {
            InitializeComponent();
            _userId = Preferences.Get("LoggedUserId", string.Empty);
            UsernameLabel.Text = $"Bienvenido, {username}";

            // 🔍 Buscar cualquier archivo de imagen guardado en la carpeta de la aplicación
            string[] imageFormats = { ".jpg", ".jpeg", ".png", ".webp" };
            foreach (var format in imageFormats)
            {
                string possibleImagePath = Path.Combine(FileSystem.AppDataDirectory, $"pp{format}");
                if (File.Exists(possibleImagePath))
                {
                    imagePath = possibleImagePath; // 🔥 Si existe un archivo con esta extensión, usarlo
                    break;
                }
            }

            Console.WriteLine($"📂 Imagen detectada en: {imagePath}");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            Console.WriteLine("🔄 Cargando perfil de usuario...");

            if (File.Exists(imagePath))
            {
                Console.WriteLine($"📷 Cargando imagen desde almacenamiento local: {imagePath}");
                ProfileImage.Source = ImageSource.FromFile(imagePath);
            }
            else
            {
                Console.WriteLine("⚠️ No se encontró imagen en la carpeta de la aplicación. Mostrando imagen predeterminada.");
                ProfileImage.Source = "default_profile.png";
            }
        }

        private async void OnChangeProfilePictureClicked(object sender, EventArgs e)
        {
            try
            {
                FileResult photo = await MediaPicker.PickPhotoAsync();
                if (photo == null)
                    return;

                // 🔹 Obtener la extensión del archivo original
                string extension = Path.GetExtension(photo.FullPath).ToLower();

                // 🔥 Generar un nombre dinámico basado en la extensión
                string imagePath = Path.Combine(FileSystem.AppDataDirectory, $"pp{extension}");

                using Stream stream = await photo.OpenReadAsync();
                using FileStream fileStream = new(imagePath, FileMode.Create, FileAccess.Write);
                await stream.CopyToAsync(fileStream);

                Console.WriteLine($"✅ Imagen guardada en: {imagePath}");
                ProfileImage.Source = ImageSource.FromFile(imagePath);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al actualizar la foto: {ex.Message}", "OK");
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Remove("LoggedUserId");
            Preferences.Remove("Username");
            Preferences.Set("IsUserLoggedIn", false);

            await DisplayAlert("Sesión cerrada", "Has cerrado sesión correctamente.", "OK");
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }

        protected override bool OnBackButtonPressed()
        {
            if (Preferences.Get("IsUserLoggedIn", false))
            {
                Application.Current.MainPage = new NavigationPage(new MainPage());
                return true;
            }

            return base.OnBackButtonPressed();
        }
    }
}