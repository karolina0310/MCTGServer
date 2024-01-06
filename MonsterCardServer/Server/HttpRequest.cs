using System;
using System.Linq;


namespace MonsterCardServer.Server
{
    public class HttpRequest
    {
        public string Content { get; set; }
        public string Method { get; set; }
        public string HttpVersion { get; set; }
        public string Route { get; set; }
        public string RouteParameter { get; set; }
        
        
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();


        public void AddHeaderPair(string key, string value)
        {
            Headers.Add(key, value);
        }
    }
}
