using CCGGame.ViewModels;
using Microsoft.Maui.Controls;

namespace CCGGame.Views
{
    public partial class DeckBuilderPage : ContentPage
    {
        public DeckBuilderPage(DeckBuilderViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}

