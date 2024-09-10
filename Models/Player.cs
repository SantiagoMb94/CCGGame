namespace CCGGame.Models
{
    public class Player
    {
        public string Name { get; set; }
        public Deck PlayerDeck { get; private set; }
        public int Health { get; set; }

        public Player(string name)
        {
            Name = name;
            PlayerDeck = new Deck();
            Health = 100; // Vida inicial del jugador
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0;
        }
    }
}
