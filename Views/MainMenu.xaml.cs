namespace CCGGame.Views;

public partial class MainMenu : ContentPage
{
    public MainMenu()
    {
        InitializeComponent();
        BindingContext = new ViewModels.MainMenuViewModel();
    }
}
