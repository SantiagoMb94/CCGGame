using System.Collections.Generic;
using CCGGame.Models;

namespace CCGGame.Services
{
    public interface ICardDataService
    {
        List<Card> GetAllCards();
        List<Card> GetCardsByType(string cardType);
        List<Card> GetCardsByRarity(string rarity);
        Card? GetCardById(int id);
        List<Card> SearchCards(string searchTerm);
    }
}
