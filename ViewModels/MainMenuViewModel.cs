using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CCGGame.Services;
using CCGGame.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CCGGame.ViewModels
{
    public class MainMenuViewModel : INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;

        public MainMenuViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            NavigateToDeckBuilderCommand = new Command(async () => await NavigateToDeckBuilder());
            NavigateToDuelCommand = new Command(async () => await NavigateToDuel());
        }

        public ICommand NavigateToDeckBuilderCommand { get; }
        public ICommand NavigateToDuelCommand { get; }

        private async Task NavigateToDeckBuilder()
        {
            var page = _serviceProvider.GetService<DeckBuilderPage>();
            if (page != null)
            {
                var navigation = Application.Current?.MainPage?.Navigation;
                if (navigation != null)
                {
                    await navigation.PushAsync(page);
                }
            }
        }

        private async Task NavigateToDuel()
        {
            var page = _serviceProvider.GetService<DuelPage>();
            if (page != null)
            {
                var navigation = Application.Current?.MainPage?.Navigation;
                if (navigation != null)
                {
                    await navigation.PushAsync(page);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
