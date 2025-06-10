using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace CCGGame.ViewModels
{
    public class MainMenuViewModel
    {
        public ICommand StartNewGameCommand { get; }
        public ICommand LoadGameCommand { get; }

        public MainMenuViewModel()
        {
            StartNewGameCommand = new Command(OnStartNewGame);
            LoadGameCommand = new Command(OnLoadGame);
        }

        private void OnStartNewGame()
        {
            // TODO: Add logic to start a new game
        }

        private void OnLoadGame()
        {
            // TODO: Add logic to load a saved game
        }
    }
}
