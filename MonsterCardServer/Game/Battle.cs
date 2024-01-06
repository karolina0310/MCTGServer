using MonsterCardServer.Database;
using MonsterCardServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardServer.Game
{
	public class Battle
	{
		public UsersDB UsersDB { get; set; }

		const int K_FACTOR = 30;

		public UserModel user1;
        public UserModel user2;
        public List<CardModel> user1Deck;
        public List<CardModel> user2Deck;
        public List<string> battleLog;
        public Random rng;

		public Battle(UserModel user1, UserModel user2, List<CardModel> deck1, List<CardModel> deck2)
		{
			UsersDB = new UsersDB();

			this.user1 = user1;
			this.user2 = user2;
			this.user1Deck = new List<CardModel>(deck1);
			this.user2Deck = new List<CardModel>(deck2);
			this.battleLog = new List<string>();
			this.rng = new Random();
		}

		public async Task RunBattle()
		{
			int round = 0;
			while (user1Deck.Count > 0 && user2Deck.Count > 0 && round < 100)
			{
				round++;
				battleLog.Add($"Round {round}:");

				CardModel card1 = user1Deck[rng.Next(user1Deck.Count)];
				CardModel card2 = user2Deck[rng.Next(user2Deck.Count)];
				battleLog.Add($"{user1.Username} plays {card1.Name}, {user2.Username} plays {card2.Name}");

				TriggerRandomEvent(); //unique feature

				if (CheckSpecialConditions(card1, card2))
				{
					continue;
				}

				double damage1 = CalculateDamage(card1, card2);
				double damage2 = CalculateDamage(card2, card1);

				if (damage1 > damage2)
				{
					WinRound(user1, user2, card1, card2);
				}
				else if (damage2 > damage1)
				{
					WinRound(user2, user1, card2, card1);
				}
				else
				{
					battleLog.Add("It's a draw! No cards are moved.");
				}
			}

			if (round >= 100)
			{
				battleLog.Add("Battle ended in a draw after 100 rounds.");
			}
			else
			{
				double expectedScoreUser1 = 1 / (1 + Math.Pow(10, (user2.Elo - user1.Elo) / 400.0));
				double expectedScoreUser2 = 1 / (1 + Math.Pow(10, (user1.Elo - user2.Elo) / 400.0));

				double scoreUser1 = user1Deck.Count > 0 ? 1 : 0;
				double scoreUser2 = user1Deck.Count > 0 ? 0 : 1;

				user1.Elo = (int)(user1.Elo + K_FACTOR * (scoreUser1 - expectedScoreUser1));
				user2.Elo = (int)(user2.Elo + K_FACTOR * (scoreUser2 - expectedScoreUser2));

				user1.GamesPlayed++;
				user2.GamesPlayed++;

				if (user1Deck.Count > 0) // user1 wins
				{
					user1.Wins++;
					user2.Loses++;
					battleLog.Add($"{user1.DisplayName} won the battle!");
				}
				else // user2 wins
				{
					user2.Wins++;
					user1.Loses++;
					battleLog.Add($"{user2.DisplayName} won the battle!");
				}

				await UsersDB.ChangeUserStats(user1);
				await UsersDB.ChangeUserStats(user2);

				Console.WriteLine("Battle ended, printing logs");
				foreach (var log in battleLog)
				{
					Console.WriteLine(log);
				}

			}

			return;
		}

		//unique feature:
		//es gibt eine 10% chance ein aus zwei zufälligen events
		//welches einen effekt auf die derzeitige runde hat
        private void TriggerRandomEvent()
        {
            if (rng.Next(100) < 10)
            {
                var eventType = rng.Next(2);
                switch (eventType)
                {
                    case 0:
                        ApplyDamageBoost();
                        break;
                    case 1:
                        ApplyElementChange();
                        break;
                }
            }
        }

        private void ApplyDamageBoost()
        {
            var targetDeck = rng.Next(2) == 0 ? user1Deck : user2Deck;
            var card = targetDeck[rng.Next(targetDeck.Count)];
            card.Damage += 5; // Zufällige Karte von einem Spieler erhält +5 Damage

            battleLog.Add($"Random Event: Damage Boost applied to {card.Name}");
        }

        private void ApplyElementChange() //ändert das Element einer zufälligen Karte eines zufälligen Spielers
        {
            var targetDeck = rng.Next(2) == 0 ? user1Deck : user2Deck;
            var card = targetDeck[rng.Next(targetDeck.Count)];

            var elements = new List<string> { "Fire", "Water", "Regular", "Earth", "Air" };

            elements.Remove(card.Element);

            var newElement = elements[rng.Next(elements.Count)];
            card.Element = newElement;

            battleLog.Add($"Random Event: Element of {card.Name} changed to {newElement}");
        }


        public bool CheckSpecialConditions(CardModel card1, CardModel card2)
		{
			if (card1.Name.Contains("Goblin") && card2.Name.Contains("Dragon"))
			{
				battleLog.Add("Goblins are too afraid of Dragons to attack. Round skipped.");
				return true;
			}
			if (card1.Name.Contains("Wizzard") && card2.Name.Contains("Ork"))
			{
				battleLog.Add("Wizzard controls Orks. Orks can't attack. Round skipped.");
				return true;
			}
			if (card1.Name.Contains("Knight") && card2.Type == "Spell" && card2.Element == "Water")
			{
				battleLog.Add("Knight drowns due to heavy armor in Water Spell. Round skipped.");
				return true;
			}
			if (card1.Name.Contains("Kraken") && card2.Type == "Spell")
			{
				battleLog.Add("Kraken is immune to spells. Round skipped.");
				return true;
			}
			if (card1.Name.Contains("FireElf") && card2.Name.Contains("Dragon"))
			{
				battleLog.Add("FireElves evade Dragon's attack. Round skipped.");
				return true;
			}

			return false;
		}

        public double CalculateDamage(CardModel attacker, CardModel defender)
        {
            double damageMultiplier = 1;

            // Spells haben elementbasierte Effekte
            if (attacker.Type == "Spell" && defender.Type == "Monster")
            {
                switch (attacker.Element)
                {
                    case "Water":
                        damageMultiplier = (defender.Element == "Fire") ? 2 : (defender.Element == "Regular") ? 0.5 : 1;
                        break;
                    case "Fire":
                        damageMultiplier = (defender.Element == "Regular") ? 2 : (defender.Element == "Water") ? 0.5 : 1;
                        break;
                    case "Regular":
                        damageMultiplier = (defender.Element == "Water") ? 2 : (defender.Element == "Fire") ? 0.5 : 1;
                        break;
                    case "Earth":
                        damageMultiplier = (defender.Element == "Fire") ? 2 : (defender.Element == "Air") ? 0.5 : 1;
                        break;
                    case "Air":
                        damageMultiplier = (defender.Element == "Water") ? 2 : (defender.Element == "Earth") ? 0.5 : 1;
                        break;
                }
            }

            return attacker.Damage * damageMultiplier;
        }




        public void WinRound(UserModel winner, UserModel loser, CardModel winningCard, CardModel losingCard)
		{
			List<CardModel> winnerDeck = (winner.Id == user1.Id) ? user1Deck : user2Deck;
			List<CardModel> loserDeck = (loser.Id == user1.Id) ? user1Deck : user2Deck;

			loserDeck.Remove(losingCard);
			winnerDeck.Add(losingCard);

			battleLog.Add($"{winner.Username} wins the round with {winningCard.Name}. {losingCard.Name} is moved to {winner.Username}'s deck.");
		}

	}
}
