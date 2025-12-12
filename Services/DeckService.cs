using CCGGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCGGame.Services
{
    public class DeckService
    {
        private const int MinDeckSize = 30;
        private const int MaxDeckSize = 40;

        public void ShuffleDeck(Deck deck)
        {
            if (deck?.Cards == null || deck.Cards.Count == 0)
                return;

            var rng = new Random();
            var shuffled = deck.Cards.OrderBy(c => rng.Next()).ToList();
            deck.Cards = shuffled;
        }

        public bool ValidateDeck(Deck deck)
        {
            if (deck == null || deck.Cards == null)
                return false;

            // Validar tamaño del mazo
            if (deck.Cards.Count < MinDeckSize || deck.Cards.Count > MaxDeckSize)
                return false;

            // Validar límites de cartas por rareza
            foreach (var cardGroup in deck.Cards.GroupBy(c => c.Id))
            {
                var card = cardGroup.First();
                var count = cardGroup.Count();

                if (card.Rarity == "Legendary" && count > 1)
                    return false;

                if (card.Rarity != "Legendary" && count > 3)
                    return false;
            }

            return true;
        }

        public List<string> GetDeckValidationErrors(Deck deck)
        {
            var errors = new List<string>();

            if (deck == null || deck.Cards == null)
            {
                errors.Add("El mazo no existe o no tiene cartas");
                return errors;
            }

            if (deck.Cards.Count < MinDeckSize)
                errors.Add($"El mazo tiene muy pocas cartas. Mínimo: {MinDeckSize}, Actual: {deck.Cards.Count}");

            if (deck.Cards.Count > MaxDeckSize)
                errors.Add($"El mazo tiene demasiadas cartas. Máximo: {MaxDeckSize}, Actual: {deck.Cards.Count}");

            // Validar límites de cartas
            foreach (var cardGroup in deck.Cards.GroupBy(c => c.Id))
            {
                var card = cardGroup.First();
                var count = cardGroup.Count();

                if (card.Rarity == "Legendary" && count > 1)
                    errors.Add($"Solo puedes tener 1 copia de '{card.Name}' (Legendaria)");

                if (card.Rarity != "Legendary" && count > 3)
                    errors.Add($"Solo puedes tener 3 copias de '{card.Name}'");
            }

            return errors;
        }

        public Deck CreateStarterDeck(CardDataService cardService)
        {
            var deck = new Deck("Mazo Inicial");
            var allCards = cardService.GetAllCards();
            var rng = new Random();

            // Agregar cartas comunes hasta llegar al mínimo
            var commonCards = allCards.Where(c => c.Rarity == "Common").OrderBy(c => rng.Next()).Take(MinDeckSize).ToList();
            
            foreach (var card in commonCards)
            {
                deck.AddCard(card);
            }

            ShuffleDeck(deck);
            return deck;
        }

        public int GetDeckCost(Deck deck)
        {
            if (deck?.Cards == null)
                return 0;

            return deck.Cards.Sum(c => c.Cost);
        }

        public double GetDeckAverageCost(Deck deck)
        {
            if (deck?.Cards == null || deck.Cards.Count == 0)
                return 0;

            return deck.Cards.Average(c => c.Cost);
        }
    }
}
