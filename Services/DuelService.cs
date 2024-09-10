using CCGGame.Models;

namespace CCGGame.Services
{
    public class DuelService
    {
        public void StartDuel(Player player1, Player player2)
        {
            // Lógica inicial para comenzar un duelo entre dos jugadores
            player1.PlayerDeck.DrawCard();
            player2.PlayerDeck.DrawCard();
        }

        public void ResolveTurn(Player player, Card playedCard, Player opponent)
        {
            // Lógica para resolver el turno de un jugador
            opponent.TakeDamage(playedCard.Attack);
        }
    }
}
