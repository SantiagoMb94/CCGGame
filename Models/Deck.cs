using System.Collections.Generic;
using System.Linq;

namespace CCGGame.Models
{
    public class Deck
    {
        public string Name { get; set; }
        public List<Card> Cards { get; set; }
        private const int MaxDeckSize = 40;
        private const int MinDeckSize = 30;

        public Deck(string name = "Nuevo Mazo")
        {
            Name = name;
            Cards = new List<Card>();
        }

        public bool AddCard(Card card)
        {
            if (Cards.Count >= MaxDeckSize)
            {
                return false; // Mazo lleno
            }

            // Limitar cartas legendarias a 1 por mazo
            if (card.Rarity == "Legendary" && Cards.Count(c => c.Id == card.Id) >= 1)
            {
                return false;
            }

            // Limitar otras cartas a 3 por mazo
            if (card.Rarity != "Legendary" && Cards.Count(c => c.Id == card.Id) >= 3)
            {
                return false;
            }

            Cards.Add(card);
            return true;
        }

        public bool RemoveCard(Card card)
        {
            return Cards.Remove(card);
        }

        public Card? DrawCard()
        {
            if (Cards.Count > 0)
            {
                var drawnCard = Cards[0];
                Cards.RemoveAt(0);
                return drawnCard;
            }
            return null;
        }

        public bool IsValid()
        {
            return Cards.Count >= MinDeckSize && Cards.Count <= MaxDeckSize;
        }

        public int GetCardCount(int cardId)
        {
            return Cards.Count(c => c.Id == cardId);
        }

        public Dictionary<string, int> GetCardTypeDistribution()
        {
            return Cards.GroupBy(c => c.CardType)
                       .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
