using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CCGGame.Models;
using CCGGame.Services;

namespace CCGGame.ViewModels
{
    public class DeckBuilderViewModel : INotifyPropertyChanged
    {
        private readonly CardDataService _cardDataService;
        private readonly DeckService _deckService;
        private string _searchText = string.Empty;
        private string _selectedCardType = "All";
        private string _selectedRarity = "All";
        private Card? _selectedCard;

        public DeckBuilderViewModel(CardDataService cardDataService, DeckService deckService)
        {
            _cardDataService = cardDataService;
            _deckService = deckService;

            AvailableCards = new ObservableCollection<Card>();
            DeckCards = new ObservableCollection<Card>();
            PlayerDeck = new Deck("Mi Mazo");

            LoadCards();
            LoadDeckCards();

            AddCardCommand = new Command<Card>(AddCardToDeck, CanAddCard);
            RemoveCardCommand = new Command<Card>(RemoveCardFromDeck);
            SearchCommand = new Command(FilterCards);
            FilterByTypeCommand = new Command<string>(FilterByType);
            FilterByRarityCommand = new Command<string>(FilterByRarity);
            SaveDeckCommand = new Command(SaveDeck, CanSaveDeck);
            ClearDeckCommand = new Command(ClearDeck);
            CreateStarterDeckCommand = new Command(CreateStarterDeck);
        }

        public ObservableCollection<Card> AvailableCards { get; set; }
        public ObservableCollection<Card> DeckCards { get; set; }
        public Deck PlayerDeck { get; set; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    FilterCards();
                }
            }
        }

        public string SelectedCardType
        {
            get => _selectedCardType;
            set
            {
                if (_selectedCardType != value)
                {
                    _selectedCardType = value;
                    OnPropertyChanged();
                    FilterCards();
                }
            }
        }

        public string SelectedRarity
        {
            get => _selectedRarity;
            set
            {
                if (_selectedRarity != value)
                {
                    _selectedRarity = value;
                    OnPropertyChanged();
                    FilterCards();
                }
            }
        }

        public Card? SelectedCard
        {
            get => _selectedCard;
            set
            {
                _selectedCard = value;
                OnPropertyChanged();
            }
        }

        public int DeckCount => PlayerDeck.Cards.Count;
        public string DeckStatus => $"{DeckCount}/40 cartas";
        public bool IsDeckValid => _deckService.ValidateDeck(PlayerDeck);
        public string ValidationMessage => IsDeckValid ? "Mazo válido" : string.Join(", ", _deckService.GetDeckValidationErrors(PlayerDeck));

        public ICommand AddCardCommand { get; }
        public ICommand RemoveCardCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand FilterByTypeCommand { get; }
        public ICommand FilterByRarityCommand { get; }
        public ICommand SaveDeckCommand { get; }
        public ICommand ClearDeckCommand { get; }
        public ICommand CreateStarterDeckCommand { get; }

        private void LoadCards()
        {
            AvailableCards.Clear();
            var cards = _cardDataService.GetAllCards();
            foreach (var card in cards)
            {
                AvailableCards.Add(card);
            }
        }

        private void LoadDeckCards()
        {
            DeckCards.Clear();
            foreach (var card in PlayerDeck.Cards)
            {
                DeckCards.Add(card);
            }
            OnPropertyChanged(nameof(DeckCount));
            OnPropertyChanged(nameof(DeckStatus));
            OnPropertyChanged(nameof(IsDeckValid));
            OnPropertyChanged(nameof(ValidationMessage));
        }

        private void AddCardToDeck(Card card)
        {
            if (card == null) return;

            if (PlayerDeck.AddCard(card))
            {
                LoadDeckCards();
                ((Command)AddCardCommand).ChangeCanExecute();
                ((Command)SaveDeckCommand).ChangeCanExecute();
            }
        }

        private void RemoveCardFromDeck(Card card)
        {
            if (card == null) return;

            if (PlayerDeck.RemoveCard(card))
            {
                LoadDeckCards();
                ((Command)AddCardCommand).ChangeCanExecute();
                ((Command)SaveDeckCommand).ChangeCanExecute();
            }
        }

        private bool CanAddCard(Card card)
        {
            if (card == null) return false;
            return PlayerDeck.Cards.Count < 40;
        }

        private void FilterCards()
        {
            AvailableCards.Clear();

            var cards = _cardDataService.GetAllCards();

            // Filtrar por búsqueda
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                cards = _cardDataService.SearchCards(SearchText);
            }

            // Filtrar por tipo
            if (SelectedCardType != "All")
            {
                cards = cards.Where(c => c.CardType == SelectedCardType).ToList();
            }

            // Filtrar por rareza
            if (SelectedRarity != "All")
            {
                cards = cards.Where(c => c.Rarity == SelectedRarity).ToList();
            }

            foreach (var card in cards)
            {
                AvailableCards.Add(card);
            }
        }

        private void FilterByType(string cardType)
        {
            SelectedCardType = cardType;
        }

        private void FilterByRarity(string rarity)
        {
            SelectedRarity = rarity;
        }

        private bool CanSaveDeck()
        {
            return IsDeckValid;
        }

        private void SaveDeck()
        {
            // Aquí se podría implementar persistencia
            // Por ahora solo validamos
            if (IsDeckValid)
            {
                // Guardar el mazo (implementar persistencia si es necesario)
            }
        }

        private void ClearDeck()
        {
            PlayerDeck.Cards.Clear();
            LoadDeckCards();
            ((Command)AddCardCommand).ChangeCanExecute();
            ((Command)SaveDeckCommand).ChangeCanExecute();
        }

        private void CreateStarterDeck()
        {
            PlayerDeck = _deckService.CreateStarterDeck(_cardDataService);
            LoadDeckCards();
            ((Command)AddCardCommand).ChangeCanExecute();
            ((Command)SaveDeckCommand).ChangeCanExecute();
        }

        public List<string> GetCardTypes()
        {
            return _cardDataService.GetAllCards()
                .Select(c => c.CardType)
                .Distinct()
                .OrderBy(t => t)
                .ToList();
        }

        public List<string> GetRarities()
        {
            return new List<string> { "All", "Common", "Rare", "Epic", "Legendary" };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
