using CCGGame.ViewModels;
using Microsoft.Maui.Controls;

namespace CCGGame.Views
{
    public partial class MainMenu : ContentPage
    {
        public MainMenu(MainMenuViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}

