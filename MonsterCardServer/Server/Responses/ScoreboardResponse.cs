using MonsterCardServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardServer.Server.Responses
{
    internal class ScoreboardResponse: HttpResponse
    {
        public ScoreboardResponse(HttpRequest? request) : base(request)
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
            try
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
                var users = await UsersDB.GetScoreboard();

                var list = users.Select(user => new
                {
                    Username = user.Username,
                    Elo = user.Elo,
                    Wins = user.Wins,
                    Losses = user.Loses
                }).ToList();

                Content = JsonConvert.SerializeObject(list);
                StatusCode = 200;
                return;

            }
            catch (Exception ex)
            {
                Content = $"Error: {ex.ToString()}";
                StatusCode = 400;

            }

            return;
        }

        public override async Task Post()
        {
            Content = "Not Implemented";
            StatusCode = 404;
            return;
        }

        public override async Task Put()
        {
            Content = "Not Implemented";
            StatusCode = 404;
            return;

        }
    }
}
