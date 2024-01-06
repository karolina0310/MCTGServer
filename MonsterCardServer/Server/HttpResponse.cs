using MonsterCardServer.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace MonsterCardServer.Server
{

    public abstract class HttpResponse
    {
        public UsersDB UsersDB { get; set; }
        public PackagesDB PackagesDB { get; set; }
        public CardsDB CardsDB { get; set; }
        public HttpRequest Request { get; set; }
        public int StatusCode { get; set; }
        public string Content { get; set; }

        public HttpResponse(HttpRequest request) //constructor for HttpResponse
        {
            UsersDB=new UsersDB();
            PackagesDB=new PackagesDB();
            CardsDB = new CardsDB();
            Request = request;
        }

        public void Send(StreamWriter writer)
        {
            try
            {
                WriteResponse(writer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured: {ex.Message}");
            }
        }

        private void WriteResponse(StreamWriter writer)
        {
            using (writer)
            {
                WriteLine(writer, $"HTTP/1.1 {StatusCode}");
                WriteLine(writer, $"Current Time: {DateTime.Now}");
                WriteLine(writer, "Server: MonsterCardTradingGame Server");

                if (Content != null)
                {
                    WriteLine(writer, $"Content-Length: {Content.Length}");
                    WriteLine(writer, "Content-Type: text/html; charset=utf-8");
                    WriteLine(writer, "");
                    WriteLine(writer, Content);
                }

                WriteLine(writer, "");
            }
        }

        private void WriteLine(StreamWriter writer, string s)
        {
            Console.WriteLine(s);
            writer.WriteLine(s);
            writer.Flush();
        }


        public abstract Task Put();
        
        public abstract Task Get();
        
        public abstract Task Post();
        
        public abstract Task Delete();

       
    }
}
