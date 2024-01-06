using NUnit.Framework;
using MonsterCardServer.Game;
using MonsterCardServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonsterCardServer.Server;
using System.Net.Sockets;
using MonsterCardServer.Server.Responses;


namespace MonsterCardServer_UnitTests
{
    [TestFixture]
    public class Tests
    {
        private Battle battle;
        private UserModel user1, user2;
        private List<CardModel> deck1, deck2;

        [SetUp]
        public void Setup()
        {
            user1 = new UserModel
            {
                Id = 1,
                Username = "HeroicGamer",
                Password = "secure123",
                Coins = 150,
                UserDeck = new List<string> { "card001", "card002" },
                Wins = 10,
                Loses = 5,
                Elo = 1100,
                GamesPlayed = 20,
                Image = "heroicgamer.jpg",
                DisplayName = "Heroic Gamer",
                Bio = "Lover of all card games."
            };
            user2 = new UserModel
            {
                Id = 2,
                Username = "MysticPlayer",
                Password = "mystic456",
                Coins = 200,
                UserDeck = new List<string> { "card003", "card004" },
                Wins = 15,
                Loses = 3,
                Elo = 1200,
                GamesPlayed = 25,
                Image = "mysticplayer.jpg",
                DisplayName = "Mystic Player",
                Bio = "Master of strategic play."
            };
            deck1 = new List<CardModel>
            {
                new CardModel { Id = "card001", Name = "Fire Dragon", Damage = 60, Element = "Fire", Type = "Monster", UserId = 1, UserName = "HeroicGamer" },
                new CardModel { Id = "card002", Name = "Ice Wizard", Damage = 30, Element = "Ice", Type = "Spell", UserId = 1, UserName = "HeroicGamer" }
            };
            deck2 = new List<CardModel>
            {
                new CardModel { Id = "card003", Name = "Earth Golem", Damage = 50, Element = "Earth", Type = "Monster", UserId = 2, UserName = "MysticPlayer" },
                new CardModel { Id = "card004", Name = "Thunder Sprite", Damage = 40, Element = "Air", Type = "Spell", UserId = 2, UserName = "MysticPlayer" }
            };

            battle = new Battle(user1, user2, deck1, deck2);
        }

        [Test]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            Assert.AreEqual(user1, battle.user1);
            Assert.AreEqual(user2, battle.user2);
            Assert.AreEqual(deck1, battle.user1Deck);
            Assert.AreEqual(deck2, battle.user2Deck);
        }

        [Test]
        public void CheckSpecialConditions_GoblinVsDragon_SkipsRound()
        {
            var goblinCard = new CardModel { Name = "Goblin" };
            var dragonCard = new CardModel { Name = "Dragon" };

            bool result = battle.CheckSpecialConditions(goblinCard, dragonCard);

            Assert.IsTrue(result);
        }

        [Test]
        public void CalculateDamage_FireSpellAgainstWaterMonster()
        {
            var fireSpell = new CardModel { Type = "Spell", Element = "Fire", Damage = 10 };
            var waterMonster = new CardModel { Type = "Monster", Element = "Water" };

            double damage = battle.CalculateDamage(fireSpell, waterMonster);

            Assert.AreEqual(5d, damage);
        }

        [Test]
        public void CalculateDamage_WaterSpellAgainstFireMonster()
        {
            var spell = new CardModel { Type = "Spell", Element = "Water", Damage = 10 };
            var monster = new CardModel { Type = "Monster", Element = "Fire" };

            double damage = battle.CalculateDamage(spell, monster);

            Assert.AreEqual(20d, damage);
        }

        [Test]
        public void WinRound_MovesCardToWinnerDeck()
        {
            var winningCard = new CardModel { Name = "Lightning Bolt", Damage = 30, Element = "Air", Type = "Spell" };
            var losingCard = new CardModel { Name = "Stone Giant", Damage = 25, Element = "Earth", Type = "Monster" };
            battle.WinRound(user1, user2, winningCard, losingCard);

            Assert.Contains(losingCard, battle.user1Deck);
            Assert.That(battle.user2Deck, Does.Not.Contain(losingCard));
        }

        [Test]
        public void EloUpdate_AfterBattle_UpdatesEloCorrectly()
        {
            // Setze anfängliche ELO-Werte
            user1.Elo = 1000;
            user2.Elo = 1000;

            // Simuliere ein Ergebnis
            battle.user1Deck.Clear(); // User 2 gewinnt
            battle.RunBattle().Wait();

            Assert.AreNotEqual(1000, user1.Elo);
            Assert.AreNotEqual(1000, user2.Elo);
        }

        [Test]
        public void RunBattle_DrawRound_NoCardMovement()
        {
            // Stellen Sie sicher, dass die Decks initialisiert sind
            var card1 = new CardModel { Damage = 30, Name = "Card1", Type = "Monster", Element = "Regular" };
            var card2 = new CardModel { Damage = 30, Name = "Card2", Type = "Monster", Element = "Regular" };
            battle.user1Deck = new List<CardModel> { card1 };
            battle.user2Deck = new List<CardModel> { card2 };

            battle.RunBattle().Wait();

            // Überprüfen, ob die Decks unverändert bleiben
            Assert.AreEqual(1, battle.user1Deck.Count);
            Assert.AreEqual(1, battle.user2Deck.Count);
        }


        [Test]
        public void CheckSpecialConditions_SkipBattleCondition_SkipsRound()
        {
            var card1 = new CardModel { Name = "Goblin" };
            var card2 = new CardModel { Name = "Dragon" };

            bool result = battle.CheckSpecialConditions(card1, card2);

            Assert.IsTrue(result);
        }


        [Test]
        public void EloUpdate_AfterWin_UpdatesEloCorrectly()
        {
            user1.Elo = 1000;
            user2.Elo = 1200;

            // Angenommen, User1 gewinnt
            battle.user1Deck = new List<CardModel> { new CardModel() };
            battle.user2Deck = new List<CardModel>();

            battle.RunBattle().Wait();

            // Überprüfen, ob User1s ELO erhöht wurde
            Assert.Greater(user1.Elo, 1000);
            Assert.Less(user2.Elo, 1200);
        }

        [Test]
        public void RunBattle_LongBattle_EndsAfterMaxRounds()
        {
            battle.user1Deck = new List<CardModel>();
            battle.user2Deck = new List<CardModel>();
            for (int i = 0; i < 100; i++)
            {
                var card1 = new CardModel("Card1", "Monster", i, "Regular", "id1", 1, "User1");
                var card2 = new CardModel("Card2", "Monster", i, "Regular", "id2", 2, "User2");
                battle.user1Deck.Add(card1);
                battle.user2Deck.Add(card2);
            }

            battle.RunBattle().Wait();

            // Überprüfen, ob das Kampfprotokoll die Begrenzung der Runden zeigt
            Assert.IsTrue(battle.battleLog.Contains("Battle ended in a draw after 100 rounds."));
        }

        [Test]
        public void Constructor_WithNullTcpClient_ShouldNotThrowExceptionAndCreateInstance()
        {
            HttpParser httpParser = null;
            Assert.DoesNotThrow(() => httpParser = new HttpParser(null));
            Assert.IsNotNull(httpParser);
        }


        [Test]
        public void ReadRequest_WithClosedTcpClient_ShouldNotCrash()
        {
            var tcpClient = new TcpClient();
            var httpParser = new HttpParser(tcpClient);
            tcpClient.Close();

            Assert.DoesNotThrow(() => httpParser.ReadRequest());
        }

        [Test]
        public void ReadRequest_WithUninitializedTcpClient_ShouldNotThrowException()
        {
            var tcpClient = new TcpClient(); // TcpClient ist nicht verbunden
            var httpParser = new HttpParser(tcpClient);

            Assert.DoesNotThrow(() => httpParser.ReadRequest());
        }


        [Test]
        public void ParseInvalidRoute_ShouldNotThrowException()
        {
            var tcpClient = new TcpClient();
            var httpParser = new HttpParser(tcpClient);

            Assert.DoesNotThrow(() => httpParser.ReadRequest());
        }

        [Test]
        public void ReadRequest_WithEmptyRequest_ShouldHandleGracefully()
        {
            var tcpClient = new TcpClient();
            var httpParser = new HttpParser(tcpClient);

            Assert.DoesNotThrow(() => httpParser.ReadRequest());
        }

        [Test]
        public async Task DeleteMethod_ShouldReturnNotImplemented()
        {
            var usersResponse = new UsersResponse(null);
            await usersResponse.Delete();
            Assert.AreEqual(404, usersResponse.StatusCode);
            Assert.AreEqual("Not Implemented", usersResponse.Content);
        }

        [Test]
        public async Task GetMethod_WithInvalidToken_ShouldReturnBadRequest()
        {
            var request = new HttpRequest();
            request.Headers["Authorization"] = "invalid-token";
            var usersResponse = new UsersResponse(request);
            await usersResponse.Get();
            Assert.AreEqual(400, usersResponse.StatusCode);

        }


        [Test]
        public async Task PostMethod_OnException_ShouldReturnBadRequest()
        {
            var request = new HttpRequest { Content = "invalid-content" };
            var usersResponse = new UsersResponse(request);
            await usersResponse.Post();
            Assert.AreEqual(400, usersResponse.StatusCode);
        }

        [Test]
        public async Task PutMethod_WithInvalidToken_ShouldReturnUnauthorized()
        {
            var request = new HttpRequest();
            request.Headers["Authorization"] = "";
            var usersResponse = new UsersResponse(request);
            await usersResponse.Put();
            Assert.AreEqual(401, usersResponse.StatusCode);
            Assert.AreEqual("Access token is missing or invalid", usersResponse.Content);
        }

        [Test]
        public void Constructor_WithHttpRequest_ShouldCreateResponse()
        {
            var request = new HttpRequest();
            var usersResponse = new UsersResponse(request);
            Assert.IsNotNull(usersResponse);
        }



    }

}