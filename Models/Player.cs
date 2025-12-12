using System.Collections.Generic;
using System.Linq;

namespace CCGGame.Models
{
    public class Player
    {
        public string Name { get; set; }
        public Deck PlayerDeck { get; private set; }
        public List<Card> Hand { get; set; }
        public List<Card> Field { get; set; } // Cartas en el campo de batalla
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
        public bool IsActive { get; set; }

        public Player(string name, Deck deck)
        {
            Name = name;
            PlayerDeck = deck;
            Hand = new List<Card>();
            Field = new List<Card>();
            MaxHealth = 100;
            Health = MaxHealth;
            MaxEnergy = 1;
            Energy = MaxEnergy;
            IsActive = false;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0;
        }

        public void Heal(int amount)
        {
            Health += amount;
            if (Health > MaxHealth) Health = MaxHealth;
        }

        public void DrawCard()
        {
            if (PlayerDeck.Cards.Count > 0 && Hand.Count < 10)
            {
                var card = PlayerDeck.DrawCard();
                if (card != null)
                {
                    Hand.Add(card);
                }
            }
        }

        public bool PlayCard(Card card)
        {
            if (!Hand.Contains(card))
                return false;

            if (!card.CanPlay(Energy))
                return false;

            Hand.Remove(card);
            Field.Add(card);
            Energy -= card.Cost;
            return true;
        }

        public void StartTurn()
        {
            IsActive = true;
            if (MaxEnergy < 10)
            {
                MaxEnergy++;
            }
            Energy = MaxEnergy;
            DrawCard();
        }

        public void EndTurn()
        {
            IsActive = false;
        }

        public bool IsDefeated()
        {
            return Health <= 0;
        }

        public int GetTotalAttackOnField()
        {
            return Field.Sum(c => c.Attack);
        }

        public int GetTotalDefenseOnField()
        {
            return Field.Sum(c => c.Defense);
        }
    }
}
