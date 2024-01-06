using MonsterCardServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;


namespace MonsterCardServer.Server.Responses
{
    internal class StatsResponse : HttpResponse
    {
        public StatsResponse(HttpRequest? request) : base(request)
        {
        }

        public override async Task Delete()
        {
            Content = "Not Implemented";
            StatusCode = 404;
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

                double winLoseRatio; //optionales feature win/lose ratio
                if (tokenUser.Loses == 0)
                {
                    if (tokenUser.Wins == 0)
                    {
                        winLoseRatio = 0; 
                    }
                    else
                    {
                        winLoseRatio = 1;
                    }
                }
                else
                {
                    winLoseRatio = (double)tokenUser.Wins / tokenUser.Loses;
                }

                var content = new
                {
                    Name = tokenUser.Username,
                    Elo = tokenUser.Elo,
                    Wins = tokenUser.Wins,
                    Losses = tokenUser.Loses,
                    WinLoseRatio = winLoseRatio
                };
                Content = JsonSerializer.Serialize(content);
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
