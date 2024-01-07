using MonsterCardServer.Database;

namespace MonsterCardServer
{
	internal class Program
	{
		static void Main(string[] args)
		{
			

			Server.Server.StartServer(10001); //startet meinen server auf port 10001
		}
	}

}
