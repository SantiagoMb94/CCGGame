using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using System.Threading.Tasks;
using CCGGame.Models;
using CCGGame.Services;

namespace CCGGame.ViewModels
{
    public class DeckBuilderViewModel : INotifyPropertyChanged
    {
        private readonly ICardDataService _cardDataService;
        private readonly IDeckService _deckService;
        private readonly ICardArtService _cardArtService;
        private string _searchText = string.Empty;
        private string _selectedCardType = "All";
        private string _selectedRarity = "All";
        private Card? _selectedCard;
        private string _saveStatus = string.Empty;

        public DeckBuilderViewModel(ICardDataService cardDataService, IDeckService deckService, ICardArtService cardArtService)
        {
            _cardDataService = cardDataService;
            _deckService = deckService;
            _cardArtService = cardArtService;

            AvailableCards = new ObservableCollection<Card>();
            DeckCards = new ObservableCollection<Card>();
            PlayerDeck = new Deck("Mi Mazo");

            LoadCards();
            LoadDeckCards();
            _ = LoadImagesAsync();

            AddCardCommand = new Command<Card>(AddCardToDeck, CanAddCard);
            RemoveCardCommand = new Command<Card>(RemoveCardFromDeck);
            SearchCommand = new Command(FilterCards);
            FilterByTypeCommand = new Command<string>(FilterByType);
            FilterByRarityCommand = new Command<string>(FilterByRarity);
            SaveDeckCommand = new Command(SaveDeck, CanSaveDeck);
            ClearDeckCommand = new Command(ClearDeck);
            CreateStarterDeckCommand = new Command(CreateStarterDeck);
            AutoFillDeckCommand = new Command(AutoFillDeck);
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
        public string SaveStatus
        {
            get => _saveStatus;
            set
            {
                if (_saveStatus != value)
                {
                    _saveStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand AddCardCommand { get; }
        public ICommand RemoveCardCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand FilterByTypeCommand { get; }
        public ICommand FilterByRarityCommand { get; }
        public ICommand SaveDeckCommand { get; }
        public ICommand ClearDeckCommand { get; }
        public ICommand CreateStarterDeckCommand { get; }
        public ICommand AutoFillDeckCommand { get; }

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

        private async Task LoadImagesAsync()
        {
            // Cargar imágenes por tipo de carta desde CardArtService (cacheado)
            var all = AvailableCards.Concat(DeckCards).ToList();
            foreach (var card in all)
            {
                card.ImageUrl = await _cardArtService.GetImageUrlForTypeAsync(card.CardType);
            }

            // Notificar refresco
            OnPropertyChanged(nameof(AvailableCards));
            OnPropertyChanged(nameof(DeckCards));
        }

        private void AddCardToDeck(Card card)
        {
            if (card == null) return;

            if (PlayerDeck.AddCard(card))
            {
                LoadDeckCards();
                ((Command)AddCardCommand).ChangeCanExecute();
                ((Command)SaveDeckCommand).ChangeCanExecute();
                SaveStatus = string.Empty;
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
                SaveStatus = string.Empty;
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
            if (IsDeckValid)
            {
                try
                {
                    var data = new
                    {
                        PlayerDeck.Name,
                        Cards = PlayerDeck.Cards.Select(c => new
                        {
                            c.Id,
                            c.Name,
                            c.Description,
                            c.Attack,
                            c.Defense,
                            c.Cost,
                            c.CardType,
                            c.Rarity
                        }).ToList()
                    };

                    var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                    var path = Path.Combine(FileSystem.AppDataDirectory, "deck_saved.json");
                    File.WriteAllText(path, json);
                    SaveStatus = $"Mazo guardado en {path}";
                }
                catch (Exception ex)
                {
                    SaveStatus = $"Error al guardar: {ex.Message}";
                }
            }
            else
            {
                SaveStatus = "El mazo no es válido, no se puede guardar.";
            }
        }

        private void ClearDeck()
        {
            PlayerDeck.Cards.Clear();
            LoadDeckCards();
            ((Command)AddCardCommand).ChangeCanExecute();
            ((Command)SaveDeckCommand).ChangeCanExecute();
            SaveStatus = string.Empty;
        }

        private void CreateStarterDeck()
        {
            PlayerDeck = _deckService.CreateStarterDeck(_cardDataService);
            LoadDeckCards();
            ((Command)AddCardCommand).ChangeCanExecute();
            ((Command)SaveDeckCommand).ChangeCanExecute();
            SaveStatus = "Mazo inicial generado.";
        }

        private void AutoFillDeck()
        {
            var rng = new Random();
            var allCards = _cardDataService.GetAllCards().OrderBy(_ => rng.Next()).ToList();

            foreach (var card in allCards)
            {
                if (PlayerDeck.Cards.Count >= 40)
                    break;

                PlayerDeck.AddCard(card);
            }

            LoadDeckCards();
            ((Command)AddCardCommand).ChangeCanExecute();
            ((Command)SaveDeckCommand).ChangeCanExecute();
            SaveStatus = "Mazo autocompletado.";
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
