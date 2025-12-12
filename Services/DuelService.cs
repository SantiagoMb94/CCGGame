using CCGGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCGGame.Services
{
    public class DuelService
    {
        private DeckService _deckService;
        public event EventHandler<string>? GameEvent;
        public event EventHandler<Player>? PlayerDefeated;

        public DuelService(DeckService deckService)
        {
            _deckService = deckService;
        }

        public void StartDuel(Player player1, Player player2)
        {
            if (player1 == null || player2 == null)
                throw new ArgumentNullException("Los jugadores no pueden ser nulos");

            // Validar mazos
            if (!_deckService.ValidateDeck(player1.PlayerDeck))
                throw new InvalidOperationException($"El mazo de {player1.Name} no es válido");

            if (!_deckService.ValidateDeck(player2.PlayerDeck))
                throw new InvalidOperationException($"El mazo de {player2.Name} no es válido");

            // Barajar mazos
            _deckService.ShuffleDeck(player1.PlayerDeck);
            _deckService.ShuffleDeck(player2.PlayerDeck);

            // Inicializar jugadores
            player1.Health = player1.MaxHealth;
            player2.Health = player2.MaxHealth;
            player1.Energy = 1;
            player2.Energy = 0;
            player1.MaxEnergy = 1;
            player2.MaxEnergy = 1;
            player1.Hand.Clear();
            player2.Hand.Clear();
            player1.Field.Clear();
            player2.Field.Clear();

            // Robar cartas iniciales (3 cartas)
            for (int i = 0; i < 3; i++)
            {
                player1.DrawCard();
                player2.DrawCard();
            }

            // El jugador 1 comienza
            player1.IsActive = true;
            player2.IsActive = false;

            OnGameEvent($"¡El duelo ha comenzado! {player1.Name} vs {player2.Name}");
        }

        public bool PlayCard(Player activePlayer, Card card, Player opponent)
        {
            if (activePlayer == null || opponent == null || card == null)
                return false;

            if (!activePlayer.IsActive)
            {
                OnGameEvent($"{activePlayer.Name} no está en su turno");
                return false;
            }

            if (!activePlayer.Hand.Contains(card))
            {
                OnGameEvent($"{activePlayer.Name} no tiene esa carta en la mano");
                return false;
            }

            if (!card.CanPlay(activePlayer.Energy))
            {
                OnGameEvent($"{activePlayer.Name} no tiene suficiente energía para jugar {card.Name}");
                return false;
            }

            // Jugar la carta
            if (activePlayer.PlayCard(card))
            {
                OnGameEvent($"{activePlayer.Name} jugó {card.Name} (Ataque: {card.Attack}, Defensa: {card.Defense})");
                return true;
            }

            return false;
        }

        public void AttackWithCard(Player attacker, Card attackingCard, Player defender, Card? defendingCard = null)
        {
            if (attacker == null || defender == null || attackingCard == null)
                return;

            if (!attacker.IsActive)
            {
                OnGameEvent($"{attacker.Name} no está en su turno");
                return;
            }

            if (!attacker.Field.Contains(attackingCard))
            {
                OnGameEvent($"{attacker.Name} no tiene {attackingCard.Name} en el campo");
                return;
            }

            int damage = attackingCard.Attack;

            if (defendingCard != null && defender.Field.Contains(defendingCard))
            {
                // Ataque contra otra carta
                OnGameEvent($"{attackingCard.Name} ataca a {defendingCard.Name}");

                defendingCard.Defense -= attackingCard.Attack;
                attackingCard.Defense -= defendingCard.Attack;

                if (defendingCard.Defense <= 0)
                {
                    defender.Field.Remove(defendingCard);
                    OnGameEvent($"{defendingCard.Name} fue destruida");
                }

                if (attackingCard.Defense <= 0)
                {
                    attacker.Field.Remove(attackingCard);
                    OnGameEvent($"{attackingCard.Name} fue destruida");
                }
            }
            else
            {
                // Ataque directo al jugador
                defender.TakeDamage(damage);
                OnGameEvent($"{attackingCard.Name} ataca directamente a {defender.Name} causando {damage} de daño");

                if (defender.IsDefeated())
                {
                    OnGameEvent($"¡{defender.Name} ha sido derrotado!");
                    PlayerDefeated?.Invoke(this, defender);
                }
            }
        }

        public void EndTurn(Player currentPlayer, Player nextPlayer)
        {
            if (currentPlayer == null || nextPlayer == null)
                return;

            currentPlayer.EndTurn();
            nextPlayer.StartTurn();

            OnGameEvent($"Turno de {currentPlayer.Name} terminado. Ahora es el turno de {nextPlayer.Name}");
        }

        public bool CheckGameOver(Player player1, Player player2)
        {
            if (player1.IsDefeated() || player2.IsDefeated())
                return true;

            // Verificar si un jugador se quedó sin cartas
            if (player1.PlayerDeck.Cards.Count == 0 && player1.Hand.Count == 0 && player1.Field.Count == 0)
            {
                OnGameEvent($"{player1.Name} se quedó sin cartas. {player2.Name} gana!");
                return true;
            }

            if (player2.PlayerDeck.Cards.Count == 0 && player2.Hand.Count == 0 && player2.Field.Count == 0)
            {
                OnGameEvent($"{player2.Name} se quedó sin cartas. {player1.Name} gana!");
                return true;
            }

            return false;
        }

        public Player? GetWinner(Player player1, Player player2)
        {
            if (player1.IsDefeated())
                return player2;

            if (player2.IsDefeated())
                return player1;

            return null;
        }

        protected virtual void OnGameEvent(string message)
        {
            GameEvent?.Invoke(this, message);
        }
    }
}

