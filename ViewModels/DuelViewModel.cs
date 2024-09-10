using CCGGame.Models;
using CCGGame.Services;

namespace CCGGame.ViewModels
{
    public class DuelViewModel
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        private DuelService duelService;

        public DuelViewModel()
        {
            Player1 = new Player("Player 1");
            Player2 = new Player("Player 2");
            duelService = new DuelService();
        }

        public void PlayTurn(Card cardPlayed)
        {
            duelService.ResolveTurn(Player1, cardPlayed, Player2);
            // Alternar entre jugadores, manejar turno, etc.
        }
    }
}
