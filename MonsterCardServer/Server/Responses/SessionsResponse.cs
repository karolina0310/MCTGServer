using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MonsterCardServer.Database;
using MonsterCardServer.Models;
using Npgsql;

namespace MonsterCardServer.Server.Responses
{
    internal class SessionsResponse : HttpResponse
    {
        public SessionsResponse(HttpRequest? request) : base(request)
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
                var user = JsonSerializer.Deserialize<UserModel>(Request.Content);
                var name = user.Username;
                var password = user.Password;

                if(name==""||password=="")
                {
                    Content = "Invalid username/password provided";
                    StatusCode = 401;
                    return;
                }

                user = await UsersDB.GetUserByUsername(name);
                
                if(!BCrypt.Net.BCrypt.Verify(password,user.Password))
                {
                    Content = "Invalid username/password provided";
                    StatusCode = 401;
                    return;
                }

                var token = $"Bearer {user.Username}-mtcgToken";
                await UsersDB.Login(token,user);

                Content = token;
                StatusCode = 200;

              
            }
            catch (PostgresException)
            {

                Content = "Invalid username/password provided";
                StatusCode = 401;
            }
            catch (Exception ex)
            {
                Content = $"Error: {ex.ToString}";
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
