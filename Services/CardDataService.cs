using CCGGame.Models;
using System.Collections.Generic;
using System.Linq;

namespace CCGGame.Services
{
    public class CardDataService
    {
        private List<Card> _allCards;

        public CardDataService()
        {
            _allCards = new List<Card>();
            InitializeCards();
        }

        private void InitializeCards()
        {
            // Cartas comunes
            _allCards.Add(new Card(1, "Guerrero Novato", "Un guerrero básico con habilidades equilibradas", 2, 2, 1, "Warrior", "Common"));
            _allCards.Add(new Card(2, "Mago Aprendiz", "Un mago joven con poder mágico básico", 1, 3, 1, "Mage", "Common"));
            _allCards.Add(new Card(3, "Arquero", "Un arquero ágil con buen ataque", 3, 1, 1, "Ranger", "Common"));
            _allCards.Add(new Card(4, "Escudero", "Un defensor con alta resistencia", 1, 4, 1, "Warrior", "Common"));
            _allCards.Add(new Card(5, "Lobo Salvaje", "Una bestia feroz", 2, 2, 1, "Beast", "Common"));
            _allCards.Add(new Card(6, "Soldado", "Un soldado entrenado", 3, 2, 2, "Warrior", "Common"));
            _allCards.Add(new Card(7, "Hechicero", "Un hechicero con poder mágico", 2, 3, 2, "Mage", "Common"));
            _allCards.Add(new Card(8, "Guardia", "Un guardia protector", 2, 4, 2, "Warrior", "Common"));
            _allCards.Add(new Card(9, "Cazador", "Un cazador experimentado", 4, 2, 2, "Ranger", "Common"));
            _allCards.Add(new Card(10, "Oso", "Una bestia poderosa", 3, 3, 2, "Beast", "Common"));

            // Cartas raras
            _allCards.Add(new Card(11, "Capitán", "Un líder experimentado", 4, 4, 3, "Warrior", "Rare"));
            _allCards.Add(new Card(12, "Archimago", "Un mago poderoso", 3, 5, 3, "Mage", "Rare"));
            _allCards.Add(new Card(13, "Maestro Arquero", "El mejor arquero", 5, 3, 3, "Ranger", "Rare"));
            _allCards.Add(new Card(14, "Dragón Joven", "Un dragón en crecimiento", 5, 4, 3, "Beast", "Rare"));
            _allCards.Add(new Card(15, "Paladín", "Un guerrero sagrado", 4, 5, 3, "Warrior", "Rare"));
            _allCards.Add(new Card(16, "Brujo", "Un mago oscuro", 6, 2, 3, "Mage", "Rare"));
            _allCards.Add(new Card(17, "Rastreador", "Un explorador experto", 4, 4, 3, "Ranger", "Rare"));
            _allCards.Add(new Card(18, "Tigre", "Un felino feroz", 5, 3, 3, "Beast", "Rare"));

            // Cartas épicas
            _allCards.Add(new Card(19, "General", "Un comandante de élite", 6, 6, 4, "Warrior", "Epic"));
            _allCards.Add(new Card(20, "Gran Mago", "Un maestro de la magia", 5, 7, 4, "Mage", "Epic"));
            _allCards.Add(new Card(21, "Asesino", "Un asesino letal", 7, 4, 4, "Ranger", "Epic"));
            _allCards.Add(new Card(22, "Dragón Adulto", "Un dragón maduro", 7, 6, 4, "Beast", "Epic"));
            _allCards.Add(new Card(23, "Caballero", "Un caballero noble", 6, 7, 4, "Warrior", "Epic"));
            _allCards.Add(new Card(24, "Necromante", "Un mago de la muerte", 8, 3, 4, "Mage", "Epic"));

            // Cartas legendarias
            _allCards.Add(new Card(25, "Rey Guerrero", "El rey de los guerreros", 8, 8, 5, "Warrior", "Legendary"));
            _allCards.Add(new Card(26, "Archimago Supremo", "El mago más poderoso", 7, 9, 5, "Mage", "Legendary"));
            _allCards.Add(new Card(27, "Dragón Anciano", "El dragón más antiguo", 9, 8, 5, "Beast", "Legendary"));
            _allCards.Add(new Card(28, "Héroe Legendario", "Un héroe de leyenda", 8, 9, 5, "Warrior", "Legendary"));
            _allCards.Add(new Card(29, "Dios de la Guerra", "Una deidad del combate", 10, 10, 6, "Warrior", "Legendary"));
            _allCards.Add(new Card(30, "Dragón Élfico", "Un dragón místico", 9, 9, 6, "Beast", "Legendary"));
        }

        public List<Card> GetAllCards()
        {
            return _allCards.ToList();
        }

        public List<Card> GetCardsByType(string cardType)
        {
            return _allCards.Where(c => c.CardType == cardType).ToList();
        }

        public List<Card> GetCardsByRarity(string rarity)
        {
            return _allCards.Where(c => c.Rarity == rarity).ToList();
        }

        public Card? GetCardById(int id)
        {
            return _allCards.FirstOrDefault(c => c.Id == id);
        }

        public List<Card> SearchCards(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllCards();

            var term = searchTerm.ToLower();
            return _allCards.Where(c => 
                c.Name.ToLower().Contains(term) || 
                c.Description.ToLower().Contains(term) ||
                c.CardType.ToLower().Contains(term)
            ).ToList();
        }
    }
}

