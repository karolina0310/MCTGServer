using MonsterCardServer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterCardServer.Game;
using System.Runtime.Intrinsics.X86;

namespace MonsterCardServer.Server.Responses
{
    internal class BattlesReponse: HttpResponse
    {
		private static ConcurrentQueue<string> playerQueue = new ConcurrentQueue<string>();

		public BattlesReponse(HttpRequest? request) : base(request)
        {
        }

        public override async Task Delete()
        {
            Content = "Not Implemented";
            StatusCode= 404;
            return;
        }

        public override async Task Get()
        {
            Content = "Not Implemented";
            StatusCode = 404;
            return;
        }

        public override async Task Post() //Battle Starten
        {
			UserModel tokenUser = new UserModel();
			var token = Request.Headers["Authorization"];

			if (token == "")
			{
				Content = "Access token is missing or invalid";
				StatusCode = 401;
				return;
			}
			tokenUser = await UsersDB.GetUserByToken(token);
			if (tokenUser == null)
			{
				Content = "Access token is missing or invalid";
				StatusCode = 401;
				return;
			}

			playerQueue.Enqueue(token);

			//Spieler kommen in eine Queue, sobald zwei Spieler drinnen sind startet das Battle
			if (playerQueue.Count >= 2)
			{
				if (playerQueue.TryDequeue(out string player1Token) && playerQueue.TryDequeue(out string player2Token))
				{
					await StartBattle(player1Token, player2Token);
				}
			}
			else
			{
				Content = "Waiting for another player to join";
				StatusCode = 202;
			}
			return;
		}

		private async Task StartBattle(string player1Token, string player2Token)
		{
			var player1 = await UsersDB.GetUserByToken(player1Token);
			var player2 = await UsersDB.GetUserByToken(player2Token);

			Battle battle = new Battle(player1, player2, await CardsDB.GetDeck(player1), await CardsDB.GetDeck(player2));
			await battle.RunBattle();
			
			Content = "Battle started between two players";
			StatusCode = 200;
		}

		public override async Task Put()
        {
            Content = "Not Implemented";
            StatusCode = 404;
            return;

        }
    }
}
