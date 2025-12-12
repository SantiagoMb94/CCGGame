namespace CCGGame.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Cost { get; set; } // Coste de energía/mana para jugar
        public string CardType { get; set; } // Ej: "Warrior", "Mage", "Spell", etc.
        public string Rarity { get; set; } // "Common", "Rare", "Epic", "Legendary"
        public string ImageUrl { get; set; }
        public List<string> Abilities { get; set; } // Habilidades especiales
        
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
            Abilities = new List<string>();
        }
        
        public Card(int id, string name, string description, int attack, int defense, int cost, string cardType, string rarity = "Common")
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
            Abilities = new List<string>();
        }

        public bool CanPlay(int availableEnergy)
        {
            return availableEnergy >= Cost;
        }

        public int GetTotalPower()
        {
            return Attack + Defense;
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
