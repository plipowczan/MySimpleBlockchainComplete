#region usings

using System;
using MySimpleBlockchainComplete.Blockchain;

#endregion

namespace MySimpleBlockchainComplete.ServerCore;

internal class Program
{
    #region Private methods

    private static void Main(string[] args)
    {
        var config = Helper.GetConfigFromFile("appsettings.json");

        var blockchain = new Blockchain.Blockchain();
        var unused = new WebServer(blockchain, config["server"], config["port"]);
        Console.WriteLine($"Serwer o adresie {config["server"]}:{config["port"]} został uruchomiony");
        Console.Read();
    }

    #endregion
}