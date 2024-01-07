using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MonsterCardServer.Models;
using Newtonsoft.Json;
using Npgsql;

namespace MonsterCardServer.Server.Responses
{
    internal class PackagesResponse: HttpResponse
    {
        public PackagesResponse(HttpRequest? request) : base(request)
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

        public async override Task Post() //erstellt ein neues Package
        {
            try
            {
                var token = Request.Headers["Authorization"];

                if (token == null) 
                {
                    Content = "Access token is missing or invalid";
                    StatusCode = 401;
                    return;
                }
                if(token != "Bearer admin-mtcgToken") 
                {
                    Content = "Provided user is not admin";
                    StatusCode = 403;
                    return;
                }
                var cards = JsonConvert.DeserializeObject<List<CardModel>>(Request.Content);
                if(await PackagesDB.AddCards(cards))
                {
                    var package = new PackageModel();
                    foreach(var card in cards) 
                    {
                        package.Cards.Add(card.Id);
                    }

                    if(await PackagesDB.CreatePackage(package))
                    {
                        Content = "Package and cards successfully created";
                        StatusCode = 201;
                        return;
                    }
                    
                    
                }
            }
            catch (PostgresException)
            {

                Content = "At least one card in the packaged already exists";
                StatusCode = 409;
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
