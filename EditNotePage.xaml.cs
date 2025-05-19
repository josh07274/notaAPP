using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;
using System.Text.Json;
using BlocDeNotas.Services;


namespace NotepadMaui
{
    public partial class EditNotePage : ContentPage
    {
        public Note CurrentNote { get; set; }

        public EditNotePage(Note note)
        {
            InitializeComponent();
            CurrentNote = note ?? new Note();
            BindingContext = CurrentNote;
        }
        protected override bool OnBackButtonPressed()
        {
            bool isLoggedIn = Preferences.Get("IsUserLoggedIn", false);

            if (isLoggedIn)
            {
                Application.Current.MainPage = new NavigationPage(new MainPage()); 
                return true; 
            }

            return base.OnBackButtonPressed(); 
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CurrentNote?.Title))
                {
                    await DisplayAlert("Error", "El título no puede estar vacío.", "OK");
                    return;
                }

                FirestoreService firestoreService = new FirestoreService();
                string userId = Preferences.Get("LoggedUserId", string.Empty);

                if (string.IsNullOrWhiteSpace(userId))
                {
                    await DisplayAlert("Error", "No se pudo recuperar el usuario.", "OK");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(CurrentNote.Id))
                {
                    // 🔥 Si la nota ya tiene un ID, actualizarla en Firestore
                    await firestoreService.UpdateNoteAsync(userId, CurrentNote.Id, CurrentNote.Title, CurrentNote.Content);
                }
                else
                {
                    // 🔥 Si no tiene ID, crear una nueva nota con un nuevo identificador
                    CurrentNote.Id = Guid.NewGuid().ToString();
                    await firestoreService.AddNoteAsync(userId, CurrentNote.Title, CurrentNote.Content);
                }

                await DisplayAlert("Guardado", "La nota se ha actualizado correctamente en Firestore.", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al guardar la nota en Firestore: {ex.Message}", "OK");
            }
        }

    }
    
}
