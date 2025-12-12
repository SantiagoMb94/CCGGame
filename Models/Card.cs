using System;
using System.Collections.Generic;
using System.Linq;

namespace CCGGame.Models
{
    public enum CardEffectTiming
    {
        Battlecry,
        Deathrattle
    }

    public enum CardEffectType
    {
        DealDamageEnemyHero,
        DealDamageRandomEnemyMinion,
        DrawCards,
        HealFriendlyHero
    }

    public class CardEffect
    {
        public CardEffectTiming Timing { get; set; }
        public CardEffectType EffectType { get; set; }
        public int Value { get; set; }
        public string? Description { get; set; }
    }

    public class Card
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Cost { get; set; } // Coste de energía/mana para jugar
        public string CardType { get; set; } // Ej: "Beast", "Dragon", "Mech", etc.
        public string Rarity { get; set; } // "Common", "Rare", "Epic", "Legendary"
        public string ImageUrl { get; set; }

        /// <summary>
        /// Palabras clave estilo HS (Taunt, Charge, DivineShield, Rush, Windfury).
        /// </summary>
        public List<string> Keywords { get; set; }

        /// <summary>
        /// Efectos simples con timing Battlecry/Deathrattle.
        /// </summary>
        public List<CardEffect> Effects { get; set; }

        public Card()
        {
            Name = string.Empty;
            Description = string.Empty;
            CardType = string.Empty;
            Rarity = "Common";
            ImageUrl = string.Empty;
            Cost = 0;
            Attack = 0;
            Defense = 0;
            Keywords = new List<string>();
            Effects = new List<CardEffect>();
        }

        public Card(int id, string name, string description, int attack, int defense, int cost, string cardType, string rarity = "Common", IEnumerable<string>? keywords = null, IEnumerable<CardEffect>? effects = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Attack = attack;
            Defense = defense;
            Cost = cost;
            CardType = cardType;
            Rarity = rarity;
            ImageUrl = string.Empty;
            Keywords = keywords?.ToList() ?? new List<string>();
            Effects = effects?.ToList() ?? new List<CardEffect>();
        }

        public bool CanPlay(int availableEnergy)
        {
            return availableEnergy >= Cost;
        }

        public int GetTotalPower()
        {
            return Attack + Defense;
        }

        public bool HasKeyword(string keyword)
        {
            return Keywords.Any(k => string.Equals(k, keyword, StringComparison.OrdinalIgnoreCase));
        }

        public string KeywordsDisplay => Keywords.Count == 0 ? string.Empty : string.Join(" • ", Keywords);

        public IEnumerable<CardEffect> GetEffects(CardEffectTiming timing)
        {
            return Effects.Where(e => e.Timing == timing);
        }

        public string GetRarityColor()
        {
            return Rarity switch
            {
                "Common" => "#808080",
                "Rare" => "#0066CC",
                "Epic" => "#9933CC",
                "Legendary" => "#FF9900",
                _ => "#808080"
            };
        }
    }
}
