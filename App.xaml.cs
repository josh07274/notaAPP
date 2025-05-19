using Bloc_de_notas;
using Microsoft.Maui.Controls;

namespace NotepadMaui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        
        bool isLoggedIn = Preferences.Get("IsUserLoggedIn", false);
        string loggedUserId = Preferences.Get("LoggedUserId", string.Empty);

        if (isLoggedIn && !string.IsNullOrWhiteSpace(loggedUserId))
        {
            MainPage = new NavigationPage(new MainPage()); 
        }
        else
        {
            MainPage = new NavigationPage(new LoginPage()); 
        }
    }

    
    public void NavigateToMainPage()
    {
        Preferences.Set("IsUserLoggedIn", true);
        MainPage = new NavigationPage(new MainPage());
    }

    
    public void Logout()
    {
        Preferences.Remove("LoggedUserId"); 
        Preferences.Remove("IsUserLoggedIn"); 
        MainPage = new NavigationPage(new LoginPage()); 
    }
}