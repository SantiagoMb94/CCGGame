using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CCGGame.Models;
using CCGGame.Services;

namespace CCGGame.ViewModels
{
    public class DuelViewModel : INotifyPropertyChanged
    {
        private readonly DuelService _duelService;
        private readonly DeckService _deckService;
        private readonly CardDataService _cardDataService;
        private Player? _activePlayer;
        private Player? _opponent;
        private Card? _selectedHandCard;
        private Card? _selectedFieldCard;
        private Card? _selectedTargetCard;
        private string _gameLog = string.Empty;
        private bool _isGameActive;
        private bool _isPlayer1Turn;

        public DuelViewModel(DuelService duelService, DeckService deckService, CardDataService cardDataService)
        {
            _duelService = duelService;
            _deckService = deckService;
            _cardDataService = cardDataService;

            Player1 = new Player("Jugador 1", new Deck("Mazo 1"));
            Player2 = new Player("Jugador 2", new Deck("Mazo 2"));

            Player1Hand = new ObservableCollection<Card>();
            Player2Hand = new ObservableCollection<Card>();
            Player1Field = new ObservableCollection<Card>();
            Player2Field = new ObservableCollection<Card>();
            GameLogMessages = new ObservableCollection<string>();

            _duelService.GameEvent += OnGameEvent;
            _duelService.PlayerDefeated += OnPlayerDefeated;

            PlayCardCommand = new Command<Card>(PlayCard, CanPlayCard);
            AttackCommand = new Command<Card>(Attack, CanAttack);
            EndTurnCommand = new Command(EndTurn, CanEndTurn);
            StartGameCommand = new Command(StartGame);
            ResetGameCommand = new Command(ResetGame);

            InitializeDecks();
        }

        public Player Player1 { get; set; }
        public Player Player2 { get; set; }

        public ObservableCollection<Card> Player1Hand { get; set; }
        public ObservableCollection<Card> Player2Hand { get; set; }
        public ObservableCollection<Card> Player1Field { get; set; }
        public ObservableCollection<Card> Player2Field { get; set; }
        public ObservableCollection<string> GameLogMessages { get; set; }

        public Player? ActivePlayer
        {
            get => _activePlayer;
            set
            {
                _activePlayer = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ActivePlayerName));
                OnPropertyChanged(nameof(ActivePlayerEnergy));
                OnPropertyChanged(nameof(ActivePlayerHealth));
            }
        }

        public Player? Opponent
        {
            get => _opponent;
            set
            {
                _opponent = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OpponentName));
                OnPropertyChanged(nameof(OpponentHealth));
            }
        }

        public Card? SelectedHandCard
        {
            get => _selectedHandCard;
            set
            {
                _selectedHandCard = value;
                OnPropertyChanged();
                ((Command)PlayCardCommand).ChangeCanExecute();
            }
        }

        public Card? SelectedFieldCard
        {
            get => _selectedFieldCard;
            set
            {
                _selectedFieldCard = value;
                OnPropertyChanged();
                ((Command)AttackCommand).ChangeCanExecute();
            }
        }

        public Card? SelectedTargetCard
        {
            get => _selectedTargetCard;
            set
            {
                _selectedTargetCard = value;
                OnPropertyChanged();
            }
        }

        public string GameLog
        {
            get => _gameLog;
            set
            {
                _gameLog = value;
                OnPropertyChanged();
            }
        }

        public bool IsGameActive
        {
            get => _isGameActive;
            set
            {
                _isGameActive = value;
                OnPropertyChanged();
                ((Command)EndTurnCommand).ChangeCanExecute();
                ((Command)StartGameCommand).ChangeCanExecute();
            }
        }

        public bool IsPlayer1Turn
        {
            get => _isPlayer1Turn;
            set
            {
                _isPlayer1Turn = value;
                OnPropertyChanged();
                UpdateActivePlayer();
            }
        }

        public string ActivePlayerName => ActivePlayer?.Name ?? "Ninguno";
        public int ActivePlayerEnergy => ActivePlayer?.Energy ?? 0;
        public int ActivePlayerHealth => ActivePlayer?.Health ?? 0;
        public string OpponentName => Opponent?.Name ?? "Ninguno";
        public int OpponentHealth => Opponent?.Health ?? 0;

        public ICommand PlayCardCommand { get; }
        public ICommand AttackCommand { get; }
        public ICommand EndTurnCommand { get; }
        public ICommand StartGameCommand { get; }
        public ICommand ResetGameCommand { get; }

        private void InitializeDecks()
        {
            // Crear mazos iniciales para ambos jugadores
            Player1.PlayerDeck = _deckService.CreateStarterDeck(_cardDataService);
            Player2.PlayerDeck = _deckService.CreateStarterDeck(_cardDataService);
        }

        private void StartGame()
        {
            try
            {
                _duelService.StartDuel(Player1, Player2);
                IsPlayer1Turn = true;
                IsGameActive = true;
                UpdateUI();
            }
            catch (Exception ex)
            {
                AddLogMessage($"Error al iniciar el juego: {ex.Message}");
            }
        }

        private void ResetGame()
        {
            InitializeDecks();
            Player1.Health = Player1.MaxHealth;
            Player2.Health = Player2.MaxHealth;
            Player1.Hand.Clear();
            Player2.Hand.Clear();
            Player1.Field.Clear();
            Player2.Field.Clear();
            GameLogMessages.Clear();
            IsGameActive = false;
            UpdateUI();
        }

        private void PlayCard(Card card)
        {
            if (ActivePlayer == null || Opponent == null || card == null)
                return;

            if (_duelService.PlayCard(ActivePlayer, card, Opponent))
            {
                UpdateUI();
                ((Command)PlayCardCommand).ChangeCanExecute();
            }
        }

        private bool CanPlayCard(Card card)
        {
            if (card == null || ActivePlayer == null)
                return false;

            return ActivePlayer.Hand.Contains(card) && card.CanPlay(ActivePlayer.Energy);
        }

        private void Attack(Card card)
        {
            if (ActivePlayer == null || Opponent == null || card == null)
                return;

            if (SelectedTargetCard != null)
            {
                _duelService.AttackWithCard(ActivePlayer, card, Opponent, SelectedTargetCard);
            }
            else
            {
                _duelService.AttackWithCard(ActivePlayer, card, Opponent);
            }

            SelectedTargetCard = null;
            UpdateUI();

            if (_duelService.CheckGameOver(Player1, Player2))
            {
                var winner = _duelService.GetWinner(Player1, Player2);
                if (winner != null)
                {
                    AddLogMessage($"¡{winner.Name} ha ganado el duelo!");
                    IsGameActive = false;
                }
            }
        }

        private bool CanAttack(Card card)
        {
            if (card == null || ActivePlayer == null)
                return false;

            return ActivePlayer.Field.Contains(card) && ActivePlayer.IsActive;
        }

        private void EndTurn()
        {
            if (ActivePlayer == null || Opponent == null)
                return;

            _duelService.EndTurn(ActivePlayer, Opponent);
            IsPlayer1Turn = !IsPlayer1Turn;
            UpdateUI();
        }

        private bool CanEndTurn()
        {
            return IsGameActive && ActivePlayer != null && ActivePlayer.IsActive;
        }

        private void UpdateActivePlayer()
        {
            ActivePlayer = IsPlayer1Turn ? Player1 : Player2;
            Opponent = IsPlayer1Turn ? Player2 : Player1;
        }

        private void UpdateUI()
        {
            Player1Hand.Clear();
            foreach (var card in Player1.Hand)
            {
                Player1Hand.Add(card);
            }

            Player2Hand.Clear();
            foreach (var card in Player2.Hand)
            {
                Player2Hand.Add(card);
            }

            Player1Field.Clear();
            foreach (var card in Player1.Field)
            {
                Player1Field.Add(card);
            }

            Player2Field.Clear();
            foreach (var card in Player2.Field)
            {
                Player2Field.Add(card);
            }

            OnPropertyChanged(nameof(ActivePlayerEnergy));
            OnPropertyChanged(nameof(ActivePlayerHealth));
            OnPropertyChanged(nameof(OpponentHealth));
        }

        private void OnGameEvent(object? sender, string message)
        {
            AddLogMessage(message);
        }

        private void OnPlayerDefeated(object? sender, Player player)
        {
            AddLogMessage($"¡{player.Name} ha sido derrotado!");
            IsGameActive = false;
        }

        private void AddLogMessage(string message)
        {
            GameLogMessages.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            GameLog = string.Join("\n", GameLogMessages);
            
            // Mantener solo los últimos 50 mensajes
            if (GameLogMessages.Count > 50)
            {
                GameLogMessages.RemoveAt(0);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
