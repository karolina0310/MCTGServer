using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using MonsterCardServer.Database;


namespace MonsterCardServer.Server
{
    static class Server
    {
        
        static TcpListener? Listener;

       
        public static void StartServer(int port)
        {
           //server auf local host gestartet
            Listener = new TcpListener(IPAddress.Loopback, port); //loopback=localhost

            Listener.Start(4); 
            Console.WriteLine("Server is waiting for requests...");
            

            while (true)
            {
                //wenn ein request geschickt wird,dann....
                if (Listener.Pending())
                {
                    //nimmt die Verbindung an und verarbeitet das Request
                    TcpClient client = Listener.AcceptTcpClient();
                    HttpParser parser = new HttpParser(client);
                    new Thread(parser.ReadRequest).Start();
                    Thread.Sleep(0);
                }

            }
        }
    }
}
