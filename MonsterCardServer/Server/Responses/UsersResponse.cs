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
    public class UsersResponse : HttpResponse
    {
        public UsersResponse(HttpRequest? request) : base(request)
        {
        }

        public override async Task Delete()
        {
            Content = "Not Implemented";
            StatusCode = 404;
            return;
        }

        public override async Task Get() // zeigt das Profil an
        {
            try
            {
                UserModel tokenUser = new UserModel();
                var token = Request.Headers["Authorization"];
                if(token != "Bearer admin-mtcgToken")
                {
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
                }
              
                var username = Request.RouteParameter;
                var user = await UsersDB.GetUserByUsername(username);

                if (user != null)
                {
                    if (user.Username == tokenUser.Username || token == "Bearer admin-mtcgToken")
                    {   
                        //wenn alles passt schicken wir den User
                        Content = JsonSerializer.Serialize(user);
                        StatusCode = 200;
                        return;
                    }
                    Content = "Access token is missing or invalid";
                    StatusCode = 401;


                }
                else
                {
                    Content = "User not found";
                    StatusCode = 404;

                }
            }
            catch (Exception ex)
            {
                Content = $"Error: {ex.ToString()}";
                StatusCode = 400;
        
            }

            return;
        }

        public override async Task Post() //Spieler erstellen
        {
            
            try
            {
                var user = JsonSerializer.Deserialize<UserModel>(Request.Content);
                if (await UsersDB.AddUser(user))
                {
                    Content = "User successfully created";
                    StatusCode = 201;
                }
                else 
                {
                    Content = "User with same username already registered";
                    StatusCode = 409;
                }
            }
            catch (PostgresException)
            {

                Content = "User with same username already registered";
                StatusCode = 409;
            }
            catch (Exception ex)
            {
                Content = $"Error: {ex.ToString()}";
                StatusCode = 400;

            }
            return;
        }

        public override async Task Put() // Spieler bearbeiten
        {
            try
            {
                UserModel tokenUser = new UserModel();
                var token = Request.Headers["Authorization"];
                if (token != "Bearer admin-mtcgToken")
                {
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
                }

                var username = Request.RouteParameter;
                var user = await UsersDB.GetUserByUsername(username);

                
            }
            catch (Exception ex)
            {
                Content = $"Error: {ex.ToString()}";
                StatusCode = 400;

            }

            return;
        }
    }
}
