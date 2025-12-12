using CCGGame.Models;
using CCGGame.Services;
using Xunit;

namespace CCGGame.Tests
{
    public class RuleTests
    {
        private Player CreatePlayer(string name)
        {
            var deck = new Deck(name + " Deck");
            return new Player(name, deck);
        }

        [Fact]
        public void Taunt_requires_targeting_taunt_first()
        {
            var duel = new DuelService(new DeckService());
            var attacker = CreatePlayer("A");
            var defender = CreatePlayer("B");

            var taunt = new Card(1, "Taunt", "", 2, 2, 1, "Beast", "Common", new[] { "Taunt" });
            defender.Field.Add(taunt);

            var attackerCard = new Card(2, "Attacker", "", 5, 5, 1, "Beast", "Common");
            attacker.Field.Add(attackerCard);
            attacker.IsActive = true;

            var defenderHp = defender.Health;
            duel.AttackWithCard(attacker, attackerCard, defender, null);

            Assert.Equal(defenderHp, defender.Health);
            Assert.Contains(taunt, defender.Field);
        }

        [Fact]
        public void DivineShield_blocks_first_damage()
        {
            var duel = new DuelService(new DeckService());
            var player = CreatePlayer("P1");
            var enemy = CreatePlayer("P2");

            var shield = new Card(3, "Shield", "", 2, 2, 1, "Mech", "Common", new[] { "DivineShield" });
            player.Hand.Add(shield);
            player.Energy = 10;
            player.IsActive = true;
            duel.PlayCard(player, shield, enemy);

            var attackerCard = new Card(4, "Attacker", "", 3, 3, 1, "Beast", "Common");
            enemy.Field.Add(attackerCard);
            enemy.IsActive = true;

            duel.AttackWithCard(enemy, attackerCard, player, shield);

            Assert.Equal(2, shield.Defense); // escudo absorbió
        }

        [Fact]
        public void Fatigue_applies_increasing_damage()
        {
            var player = CreatePlayer("P");
            player.PlayerDeck.Cards.Clear();
            player.Health = 10;

            player.DrawCard(); // 1
            player.DrawCard(); // 2

            Assert.Equal(7, player.Health);
            Assert.Equal(2, player.FatigueCounter);
        }
    }
}
