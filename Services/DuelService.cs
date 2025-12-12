using CCGGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCGGame.Services
{
    public class DuelService
    {
        private readonly DeckService _deckService;
        private readonly Random _rng = new();
        private readonly Dictionary<Card, int> _attacksUsed = new();
        private readonly HashSet<Card> _summoningSickness = new();
        private readonly HashSet<Card> _divineShields = new();
        private const int MaxFieldSize = 7;

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

            ResetBoard(player1);
            ResetBoard(player2);

            // Vida estilo HS
            player1.MaxHealth = 30;
            player2.MaxHealth = 30;
            player1.Health = player1.MaxHealth;
            player2.Health = player2.MaxHealth;

            // Maná inicial
            player1.MaxEnergy = 1;
            player2.MaxEnergy = 1;
            player1.Energy = 1;
            player2.Energy = 0;

            // Robar cartas iniciales (3)
            for (int i = 0; i < 3; i++)
            {
                player1.DrawCard();
                player2.DrawCard();
            }

            // Estado de turno
            player1.IsActive = true;
            player2.IsActive = false;

            RefreshAttacks(player1);
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
                OnGameEvent($"{activePlayer.Name} no tiene suficiente maná para jugar {card.Name}");
                return false;
            }

            if (activePlayer.Field.Count >= MaxFieldSize)
            {
                OnGameEvent("Tu campo está lleno (7). No puedes jugar más esbirros.");
                return false;
            }

            if (activePlayer.PlayCard(card))
            {
                _attacksUsed[card] = 0;
                if (!card.HasKeyword("Charge"))
                {
                    _summoningSickness.Add(card);
                }
                if (card.HasKeyword("DivineShield"))
                {
                    _divineShields.Add(card);
                }

                OnGameEvent($"{activePlayer.Name} jugó {card.Name} (ATQ {card.Attack} / VIDA {card.Defense})");

                // Resolver Grito de Batalla
                ResolveEffects(card, CardEffectTiming.Battlecry, activePlayer, opponent);
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

            // Verificar invocación (no atacar si está mareado)
            if (_summoningSickness.Contains(attackingCard) && !attackingCard.HasKeyword("Charge") && !attackingCard.HasKeyword("Rush"))
            {
                OnGameEvent($"{attackingCard.Name} no puede atacar este turno (nuevo en mesa)");
                return;
            }

            // Verificar ataques disponibles (Windfury = 2)
            var maxAttacks = attackingCard.HasKeyword("Windfury") ? 2 : 1;
            if (_attacksUsed.TryGetValue(attackingCard, out var used) && used >= maxAttacks)
            {
                OnGameEvent($"{attackingCard.Name} ya atacó el máximo permitido este turno");
                return;
            }

            // Taunt: si hay provocar enemigo, debes atacar a uno de ellos
            var taunts = defender.Field.Where(c => c.HasKeyword("Taunt")).ToList();
            if (taunts.Any())
            {
                if (defendingCard == null || !defendingCard.HasKeyword("Taunt"))
                {
                    defendingCard = taunts.First();
                    OnGameEvent("Hay Provocar en mesa rival. Debes atacarlos primero.");
                }
            }

            // Rush no puede atacar héroe en primer turno
            if (attackingCard.HasKeyword("Rush") && _summoningSickness.Contains(attackingCard) && defendingCard == null)
            {
                OnGameEvent($"{attackingCard.Name} (Rush) solo puede atacar esbirros este turno.");
                return;
            }

            if (defendingCard != null && defender.Field.Contains(defendingCard))
            {
                OnGameEvent($"{attackingCard.Name} ataca a {defendingCard.Name}");
                bool defenderDestroyed = ApplyDamageToCard(defendingCard, attackingCard.Attack, defender);
                bool attackerDestroyed = ApplyDamageToCard(attackingCard, defendingCard.Attack, attacker);

                if (defenderDestroyed)
                {
                    HandleDeath(defender, defendingCard, attacker);
                }

                if (attackerDestroyed)
                {
                    HandleDeath(attacker, attackingCard, defender);
                }
            }
            else
            {
                // Ataque directo al héroe (si no hay taunt)
                if (taunts.Any())
                {
                    OnGameEvent("No puedes atacar al héroe mientras haya esbirros con Provocar.");
                    return;
                }

                defender.TakeDamage(attackingCard.Attack);
                OnGameEvent($"{attackingCard.Name} ataca directamente a {defender.Name} causando {attackingCard.Attack} de daño");

                if (defender.IsDefeated())
                {
                    OnGameEvent($"¡{defender.Name} ha sido derrotado!");
                    PlayerDefeated?.Invoke(this, defender);
                }
            }

            _attacksUsed[attackingCard] = (_attacksUsed.TryGetValue(attackingCard, out var count) ? count : 0) + 1;
        }

        public void EndTurn(Player currentPlayer, Player nextPlayer)
        {
            if (currentPlayer == null || nextPlayer == null)
                return;

            currentPlayer.EndTurn();
            nextPlayer.StartTurn();
            RefreshAttacks(nextPlayer);

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

        private bool ApplyDamageToCard(Card target, int damage, Player owner)
        {
            if (_divineShields.Contains(target))
            {
                _divineShields.Remove(target);
                OnGameEvent($"{target.Name} pierde Escudo Divino (sin daño)");
                return false;
            }

            target.Defense -= damage;
            return target.Defense <= 0;
        }

        private void HandleDeath(Player owner, Card deadCard, Player opponent)
        {
            if (owner.Field.Contains(deadCard))
            {
                owner.Field.Remove(deadCard);
            }

            _attacksUsed.Remove(deadCard);
            _summoningSickness.Remove(deadCard);
            _divineShields.Remove(deadCard);

            OnGameEvent($"{deadCard.Name} fue destruido");
            ResolveEffects(deadCard, CardEffectTiming.Deathrattle, owner, opponent);
        }

        private void ResolveEffects(Card card, CardEffectTiming timing, Player owner, Player opponent)
        {
            foreach (var effect in card.GetEffects(timing))
            {
                switch (effect.EffectType)
                {
                    case CardEffectType.DealDamageEnemyHero:
                        opponent.TakeDamage(effect.Value);
                        OnGameEvent($"{card.Name}: {effect.Description ?? $"Hace {effect.Value} de daño al héroe enemigo"}");
                        if (opponent.IsDefeated())
                        {
                            PlayerDefeated?.Invoke(this, opponent);
                        }
                        break;
                    case CardEffectType.DealDamageRandomEnemyMinion:
                        if (opponent.Field.Any())
                        {
                            var target = opponent.Field[_rng.Next(opponent.Field.Count)];
                            bool destroyed = ApplyDamageToCard(target, effect.Value, opponent);
                            OnGameEvent($"{card.Name}: {effect.Description ?? $"Hace {effect.Value} de daño a {target.Name}"}");
                            if (destroyed)
                            {
                                HandleDeath(opponent, target, owner);
                            }
                        }
                        break;
                    case CardEffectType.DrawCards:
                        for (int i = 0; i < effect.Value; i++)
                        {
                            owner.DrawCard();
                        }
                        OnGameEvent($"{card.Name}: robas {effect.Value} carta(s)");
                        break;
                    case CardEffectType.HealFriendlyHero:
                        owner.Heal(effect.Value);
                        OnGameEvent($"{card.Name}: curas {effect.Value} a tu héroe");
                        break;
                }
            }
        }

        private void RefreshAttacks(Player player)
        {
            foreach (var card in player.Field.ToList())
            {
                _attacksUsed[card] = 0;
                _summoningSickness.Remove(card);
            }
        }

        private void ResetBoard(Player player)
        {
            player.Hand.Clear();
            player.Field.Clear();
            _attacksUsed.Clear();
            _summoningSickness.Clear();
            _divineShields.Clear();
        }

        protected virtual void OnGameEvent(string message)
        {
            GameEvent?.Invoke(this, message);
        }
    }
}

