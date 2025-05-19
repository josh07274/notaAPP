using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using BlocDeNotas.Services;
namespace NotepadMaui;

public partial class MainPage : ContentPage
{
    private readonly FirestoreService _firestoreService = new FirestoreService();
    public ObservableCollection<Note> Notes { get; set; } = new ObservableCollection<Note>();


    public MainPage()
    {
        InitializeComponent();


        NotesCollectionView.ItemsSource = Notes;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        string userId = Preferences.Get("LoggedUserId", string.Empty); // 🔥 Obtener el ID del usuario logueado

        if (string.IsNullOrWhiteSpace(userId))
        {
            await DisplayAlert("Error", "No se pudo recuperar el usuario.", "OK");
            return;
        }

        var loadedNotes = await _firestoreService.GetUserNotesAsync(userId); // 🔥 Cargar las notas desde Firestore

        Notes.Clear();
        foreach (var note in loadedNotes)
        {
            Notes.Add(new Note
            {
                Id = note.ContainsKey("Id") ? note["Id"].ToString() : Guid.NewGuid().ToString(),
                Title = note.ContainsKey("Title") ? note["Title"].ToString() : "",
                Content = note.ContainsKey("Content") ? note["Content"].ToString() : "",
                Date = note.ContainsKey("Date") ? note["Date"].ToString() : ""
            });
        }

        NotesCollectionView.ItemsSource = Notes; // 🔄 Refrescar la UI con las notas obtenidas
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

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());


    }

    private async void OnAddNoteClicked(object sender, EventArgs e)
    {
        // Simplemente navegar a la página de edición sin crear una nota vacía
        await Navigation.PushAsync(new EditNotePage(null)); // Pasamos "null" porque la nota aún no existe
    }

    private async void OnDeleteNoteClicked(object sender, EventArgs e)
    {
        try
        {
            var swipeItem = sender as SwipeItem;
            if (swipeItem?.CommandParameter is Note noteToDelete)
            {
                bool confirm = await DisplayAlert("Confirmar eliminación",
                    $"¿Deseas eliminar la nota '{noteToDelete.Title}'?", "Sí", "No");

                if (confirm)
                {
                    string userId = Preferences.Get("LoggedUserId", string.Empty); // 🔥 Obtener el ID del usuario

                    if (!string.IsNullOrWhiteSpace(userId))
                    {
                        await _firestoreService.DeleteNoteAsync(userId, noteToDelete.Id); // 🔥 Eliminar nota en Firestore
                        Notes.Remove(noteToDelete); // 🔄 Actualizar la UI
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al eliminar la nota: {ex.Message}", "OK");
        }
    }

    private async void OnNoteTapped(object sender, EventArgs e)
    {
        try
        {
            var element = sender as Element;
            if (element?.BindingContext is Note selectedNote)
            {
                await Navigation.PushAsync(new EditNotePage(selectedNote));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al abrir la nota: {ex.Message}", "OK");
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var keyword = e.NewTextValue?.ToLower() ?? "";
        NotesCollectionView.ItemsSource = string.IsNullOrWhiteSpace(keyword)
            ? Notes
            : new ObservableCollection<Note>(Notes.Where(n => n.Title.ToLower().Contains(keyword) || n.Content.ToLower().Contains(keyword)));
    }

}

public class Note
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Preview => Content.Length > 50 ? Content.Substring(0, 50) + "..." : Content;
}
