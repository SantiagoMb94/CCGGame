using System.Collections.ObjectModel;
using CCGGame.Models;

namespace CCGGame.ViewModels
{
    public class DeckBuilderViewModel
    {
        public ObservableCollection<Card> AvailableCards { get; set; }
        public Deck PlayerDeck { get; set; }

        public DeckBuilderViewModel()
        {
            AvailableCards = new ObservableCollection<Card>();
            PlayerDeck = new Deck();

            // Lógica para cargar cartas disponibles
        }

        public void AddCardToDeck(Card card)
        {
            PlayerDeck.AddCard(card);
        }
    }
}
