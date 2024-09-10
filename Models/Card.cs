namespace CCGGame.Models
{
    public class Card
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public string CardType { get; set; } // Ej: "Warrior", "Mage", etc.
        
        public Card(string name, string description, int attack, int defense, string cardType)
        {
            Name = name;
            Description = description;
            Attack = attack;
            Defense = defense;
            CardType = cardType;
        }

        public void PlayCard()
        {
            // Lógica de qué sucede cuando la carta se juega
        }
    }
}
