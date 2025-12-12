using System.Collections.Generic;
using CCGGame.Models;

namespace CCGGame.Services
{
    public interface IDeckService
    {
        void ShuffleDeck(Deck deck);
        bool ValidateDeck(Deck deck);
        List<string> GetDeckValidationErrors(Deck deck);
        Deck CreateStarterDeck(ICardDataService cardService);
        int GetDeckCost(Deck deck);
        double GetDeckAverageCost(Deck deck);
    }
}
using System.Collections.Generic;
using CCGGame.Models;

namespace CCGGame.Services
{
    public interface IDeckService
    {
        void ShuffleDeck(Deck deck);
        bool ValidateDeck(Deck deck);
        List<string> GetDeckValidationErrors(Deck deck);
        Deck CreateStarterDeck(ICardDataService cardService);
        int GetDeckCost(Deck deck);
        double GetDeckAverageCost(Deck deck);
    }
}
