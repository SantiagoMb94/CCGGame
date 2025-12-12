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
            _allCards = new List<Card>
            {
                new Card(1, "Escudero de Villanorte", "Provocar básico para proteger al héroe.", 1, 3, 1, "Human", "Common", new []{"Taunt"}),
                new Card(2, "Mago de Chispas", "Grito de batalla: 1 de daño al héroe enemigo.", 2, 1, 1, "Mage", "Common", effects: new []
                {
                    new CardEffect { Timing = CardEffectTiming.Battlecry, EffectType = CardEffectType.DealDamageEnemyHero, Value = 1, Description = "1 de daño al héroe enemigo" }
                }),
                new Card(3, "Gnomo Ingeniero", "Grito de batalla: roba 1 carta.", 1, 2, 1, "Mech", "Common", effects: new []
                {
                    new CardEffect { Timing = CardEffectTiming.Battlecry, EffectType = CardEffectType.DrawCards, Value = 1, Description = "Roba 1 carta" }
                }),
                new Card(4, "Lobo del Bosque", "Bestia agresiva.", 2, 2, 1, "Beast", "Common"),
                new Card(5, "Corredor Ágil", "Carga: puede atacar el turno que se juega.", 3, 2, 2, "Human", "Common", new []{"Charge"}),
                new Card(6, "Gólem Ancestral", "Escudo divino: ignora el primer daño recibido.", 2, 2, 2, "Elemental", "Common", new []{"DivineShield"}),
                new Card(7, "Sanador de la Luz", "Grito de batalla: cura 2 a tu héroe.", 2, 3, 2, "Priest", "Common", effects: new []
                {
                    new CardEffect { Timing = CardEffectTiming.Battlecry, EffectType = CardEffectType.HealFriendlyHero, Value = 2, Description = "Cura 2 a tu héroe" }
                }),
                new Card(8, "Arpía del Viento", "Viento furioso: puede atacar dos veces por turno.", 2, 3, 3, "Beast", "Common", new []{"Windfury"}),
                new Card(9, "Elemental de Roca", "Provocar robusto.", 3, 4, 3, "Elemental", "Common", new []{"Taunt"}),
                new Card(10, "Bruja de Hielo", "Grito de batalla: 2 de daño a un esbirro enemigo aleatorio.", 3, 3, 3, "Mage", "Common", effects: new []
                {
                    new CardEffect { Timing = CardEffectTiming.Battlecry, EffectType = CardEffectType.DealDamageRandomEnemyMinion, Value = 2, Description = "2 de daño a esbirro enemigo aleatorio" }
                }),
                new Card(11, "Vigía del Muro", "Provocar + escudo para la línea frontal.", 1, 6, 4, "Human", "Rare", new []{"Taunt", "DivineShield"}),
                new Card(12, "Dracónido de Carga", "Embiste inmediatamente con Rush.", 5, 4, 4, "Dragon", "Rare", new []{"Rush"}),
                new Card(13, "Chamán del Manantial", "Grito de batalla: cura 4 a tu héroe.", 4, 4, 4, "Shaman", "Rare", effects: new []
                {
                    new CardEffect { Timing = CardEffectTiming.Battlecry, EffectType = CardEffectType.HealFriendlyHero, Value = 4, Description = "Cura 4 a tu héroe" }
                }),
                new Card(14, "Artillero de Vapor", "Grito de batalla: 3 de daño al héroe enemigo.", 5, 5, 5, "Mech", "Rare", effects: new []
                {
                    new CardEffect { Timing = CardEffectTiming.Battlecry, EffectType = CardEffectType.DealDamageEnemyHero, Value = 3, Description = "3 de daño al héroe enemigo" }
                }),
                new Card(15, "Fénix Renacido", "Último aliento: 2 de daño a un esbirro enemigo aleatorio.", 5, 3, 5, "Beast", "Epic", effects: new []
                {
                    new CardEffect { Timing = CardEffectTiming.Deathrattle, EffectType = CardEffectType.DealDamageRandomEnemyMinion, Value = 2, Description = "2 de daño a esbirro enemigo aleatorio" }
                }),
                new Card(16, "Guardián del Vacío", "Provocar enorme para frenar al rival.", 6, 8, 6, "Demon", "Epic", new []{"Taunt"}),
                new Card(17, "Dragón del Alba", "Rush + Escudo divino: limpia la mesa rival.", 6, 6, 6, "Dragon", "Epic", new []{"Rush", "DivineShield"}),
                new Card(18, "Titán de Acero", "Provocar y escudo: ancla el final de partida.", 8, 8, 8, "Mech", "Legendary", new []{"Taunt", "DivineShield"}),
                new Card(19, "Ancestro Terrano", "Último aliento: 3 de daño al héroe enemigo.", 7, 7, 7, "Elemental", "Legendary", effects: new []
                {
                    new CardEffect { Timing = CardEffectTiming.Deathrattle, EffectType = CardEffectType.DealDamageEnemyHero, Value = 3, Description = "3 de daño al héroe enemigo" }
                }),
                new Card(20, "Campeona Atronadora", "Carga y Viento furioso: remate veloz.", 5, 5, 7, "Warrior", "Legendary", new []{"Charge", "Windfury"})
            };
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

