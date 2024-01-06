using MonsterCardServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardServer.Server.Responses
{
    internal class TransactionsResponse : HttpResponse
    {
        public TransactionsResponse(HttpRequest? request) : base(request)
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
            Content = "Not Implemented";
            StatusCode = 404;
            return;
        }

        public override async Task Post()
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
                if(tokenUser.Coins<5)
                {
                    Content = "Not enough money for buying a card package";
                    StatusCode = 403;
                    return;
                }
                if(await UsersDB.BuyPackage(tokenUser)) //todo return cards bought
                {
                    
                    Content = "Packages bought";
                    StatusCode = 200;
                    return;
                }
                else
                {
                    Content = "No card package available for buying";
                    StatusCode = 404;
                    return;
                }

              
            }
            catch (Exception ex)
            {
                Content = $"Error: {ex.ToString()}";
                StatusCode = 400;

            }

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
