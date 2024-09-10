using System.Collections.Generic;

namespace CCGGame.Models
{
    public class Deck
    {
        public List<Card> Cards { get; set; } // Cambia el set a público

        public Deck()
        {
            Cards = new List<Card>();
        }

        public void AddCard(Card card)
        {
            if (Cards.Count < 40) // Limite de 40 cartas por mazo
            {
                Cards.Add(card);
            }
        }

        public void RemoveCard(Card card)
        {
            Cards.Remove(card);
        }

        public Card DrawCard()
        {
            if (Cards.Count > 0)
            {
                var drawnCard = Cards[0];
                Cards.RemoveAt(0);
                return drawnCard;
            }
            return null;
        }
    }
}
