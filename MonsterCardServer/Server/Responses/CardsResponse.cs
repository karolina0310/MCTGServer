using MonsterCardServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardServer.Server.Responses
{
    internal class CardsResponse : HttpResponse
    {
        public CardsResponse(HttpRequest? request) : base(request)
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
                var cards = await CardsDB.GetCards(tokenUser);
                if(cards.Count()==0)
                {
                    Content = "The request was fine, but the user doesn't have any cards";
                    StatusCode = 204;
                    return;
                }
                Content = JsonConvert.SerializeObject(cards);
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
