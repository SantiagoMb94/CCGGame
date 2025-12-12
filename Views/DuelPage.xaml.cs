using CCGGame.ViewModels;
using Microsoft.Maui.Controls;

namespace CCGGame.Views
{
    public partial class DuelPage : ContentPage
    {
        public DuelPage(DuelViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}

