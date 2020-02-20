#region usings

using System;
using Microsoft.Extensions.Configuration;
using MySimpleBlockchainComplete.Blockchain;

#endregion

namespace MySimpleBlockchainComplete.ServerCore
{
    class Program
    {
        #region Private methods

        static void Main(string[] args)
        {
            IConfiguration config = Helper.GetConfigFromFile("appsettings.json");

            var blockchain = new Blockchain.Blockchain();
            var unused = new WebServer(blockchain, config["server"], config["port"]);
            Console.WriteLine($"Serwer o adresie {config["server"]}:{config["port"]} został uruchomiony");
            Console.Read();
        }

        #endregion
    }
}