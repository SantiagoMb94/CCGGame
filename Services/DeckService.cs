using CCGGame.Models;
using System;
using System.Linq;

namespace CCGGame.Services
{
    public class DeckService
    {
        public void ShuffleDeck(Deck deck)
        {
            var rng = new Random();
            deck.Cards = deck.Cards.OrderBy(c => rng.Next()).ToList();
        }

        public bool ValidateDeck(Deck deck)
        {
            return deck.Cards.Count == 40; // Valida si el mazo tiene exactamente 40 cartas
        }
    }
}
