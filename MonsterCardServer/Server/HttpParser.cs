using MonsterCardServer.Server;
using MonsterCardServer.Server.Responses;
using Npgsql.Internal;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MonsterCardServer.Server
{
    public class HttpParser
    {
        public HttpRequest _request = new HttpRequest();
        public HttpResponse _response;
        public TcpClient _socket;


        public HttpParser(TcpClient socket)
        {
            this._socket = socket;
        }



        public async void ReadRequest()
        {
            using (var writer = new StreamWriter(_socket.GetStream()) { AutoFlush = true })
            using (var reader = new StreamReader(_socket.GetStream()))
            {
                Console.WriteLine();

                string line;
                while ((line = reader.ReadLine()) != null) //geht durch jede Zeile des Requests
                {

                    if (line.Length == 0)
                        break;

                    Console.WriteLine(line);

                    //baue mein request
                    if (_request.Method == null) //setzt Methode Route und Version (erste Zeile)
                    {
                        var parts = line.Split(' ');
                        _request.Method = parts[0];
                        _request.Route = parts[1];
                        _request.HttpVersion = parts[2];
                    }
                    else //restlicher Header
                    {
                        var parts = line.Split(": ");
                        _request.AddHeaderPair(parts[0].TrimEnd(':'), parts[1]);
                    }
                }

                //setzt eventuell einen zusätzlichen Parameter der in der Route steht (Bsp users/{username})
                string[] routeParts = _request.Route.Split('/');

                
                if (routeParts.Length > 2)
                {
                    _request.Route = routeParts[1];
                    _request.RouteParameter = routeParts[2];
                }
                else if (routeParts.Length > 1)
                {
                    _request.Route = routeParts[1];
                    _request.RouteParameter = string.Empty;
                }


                Console.WriteLine(_request.Route);

                //Body auslesen
                if (_request.Headers.TryGetValue("Content-Length", out string contentLength) && int.TryParse(contentLength, out int length) && length > 0)
                {
                    char[] buffer = new char[length];
                    reader.Read(buffer, 0, length);
                    string content_string = new(buffer);
                    _request.Content = content_string;
                }

                //jenach Route wird ein bestimmter Response erstellt
                switch (_request.Route)
                {
                    case "users":
                        _response = new UsersResponse(_request);
                        break;
                    case "sessions":
                        _response = new SessionsResponse(_request);
                        break;
                    case "packages":
                        _response = new PackagesResponse(_request);
                        break;
                    case "transactions":
                        _response = new TransactionsResponse(_request);
                        break;
                    case "cards":
                        _response = new CardsResponse(_request);
                        break;
                    case "deck":
                        _response = new DeckResponse(_request);
                        break;
                    case "stats":
                        _response = new StatsResponse(_request);
                        break;
                    case "scoreboard":
                        _response = new ScoreboardResponse(_request);
                        break;
					case "battles":
						_response = new BattlesReponse(_request);
						break;
                    case "admin":
                        _response = new AdminResponse(_request);
                        break;
                    default:
                        _response = new DefaultResponse(_request);
                        break;
                }
                //ruft die richtige Methode in der neuen Response auf
                switch (_request.Method)
                {
                    case "POST":
                        await _response.Post();
                        break;
                    case "DELETE":
                        await _response.Delete();
                        break;
                    case "GET":
                        await _response.Get();
                        break;
                    case "PUT":
                        await _response.Put();
                        break;

                    default:
                        break;
                }
                //Response an den client zurückschicken
                _response.Send(writer);
            }
        }

    }
}