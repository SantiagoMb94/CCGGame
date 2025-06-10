using CCGGame.Models;
using CCGGame.Services;

namespace CCGGame.ViewModels
{
    public class DuelViewModel
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        private DuelService duelService;
        private bool isPlayer1Turn;

        public DuelViewModel()
        {
            Player1 = new Player("Player 1");
            Player2 = new Player("Player 2");
            duelService = new DuelService();
            isPlayer1Turn = true;
        }

        public void PlayTurn(Card cardPlayed)
        {
            if (isPlayer1Turn)
            {
                duelService.ResolveTurn(Player1, cardPlayed, Player2);
            }
            else
            {
                duelService.ResolveTurn(Player2, cardPlayed, Player1);
            }

            // Alternar entre jugadores
            isPlayer1Turn = !isPlayer1Turn;
        }
    }
}
