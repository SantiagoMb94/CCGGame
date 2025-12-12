using System;
using CCGGame.Models;

namespace CCGGame.Services
{
    public enum DuelPhase
    {
        NotStarted,
        Mulligan,
        TurnStart,
        Main,
        Combat,
        TurnEnd,
        Finished
    }

    public interface IDuelService
    {
        event EventHandler<string>? GameEvent;
        event EventHandler<Player>? PlayerDefeated;
        DuelPhase Phase { get; }
        int TurnNumber { get; }
        void StartDuel(Player player1, Player player2);
        bool PlayCard(Player activePlayer, Card card, Player opponent);
        void AttackWithCard(Player attacker, Card attackingCard, Player defender, Card? defendingCard = null);
        void EndTurn(Player currentPlayer, Player nextPlayer);
        bool CheckGameOver(Player player1, Player player2);
        Player? GetWinner(Player player1, Player player2);
    }
}
