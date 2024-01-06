using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardServer.Server.Responses
{
    internal class AdminResponse: HttpResponse
    {
        public AdminResponse(HttpRequest? request) : base(request)
        {
        }

        public override async Task Delete()
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
                if (token != "Bearer admin-mtcgToken")
                {
                    Content = "Provided user is not admin";
                    StatusCode = 403;
                    return;
                }
               
                await UsersDB.TruncateAllTables();
                Content = "OK";
                StatusCode = 200;
                return;
                
            }
            catch (PostgresException)
            {

                Content = "Err";
                StatusCode = 409;
            }
            catch (Exception ex)
            {

                Content = $"Error: {ex.ToString()}";
                StatusCode = 400;
            }
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
