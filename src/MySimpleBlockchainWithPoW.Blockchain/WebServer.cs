#region usings

using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

#endregion

namespace MySimpleBlockchainWithPoW.Blockchain
{
    public class WebServer
    {
        #region Constructors

        public WebServer(Blockchain blockchain, string host, string port)
        {
            var server = new TinyWebServer.WebServer(request =>
                                                     {
                                                         string path = request.Url.PathAndQuery.ToLower();
                                                         string json;
                                                         if (path.Contains("?"))
                                                         {
                                                             string[] parts = path.Split('?');
                                                             path = parts[0];
                                                         }

                                                         switch (path)
                                                         {
                                                             //GET: http://localhost:12345/mine
                                                             case "/mine":
                                                                 return blockchain.Mine();

                                                             //POST: http://localhost:12345/transactions/new
                                                             //{ "Amount":1, "From":"x", "To":"y" }
                                                             case "/transactions/new":
                                                                 if (request.HttpMethod != HttpMethod.Post.Method)
                                                                     return
                                                                         $"{new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)}";

                                                                 json = new StreamReader(request.InputStream)
                                                                     .ReadToEnd();
                                                                 Transaction trx =
                                                                     JsonConvert.DeserializeObject<Transaction>(json);
                                                                 int blockId =
                                                                     blockchain.CreateTransaction(
                                                                         trx.From, trx.To, trx.Amount);
                                                                 return
                                                                     $"Twoja transakcja zostanie dodana do bloku {blockId}";

                                                             //GET: http://localhost:12345/blockchain
                                                             case "/blockchain":
                                                                 return blockchain.GetFullChain();

                                                             //POST: http://localhost:12345/nodes/register
                                                             //{ "Url": "localhost:54321" }
                                                             case "/nodes/register":
                                                                 if (request.HttpMethod != HttpMethod.Post.Method)
                                                                     return
                                                                         $"{new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)}";

                                                                 json = new StreamReader(request.InputStream)
                                                                     .ReadToEnd();
                                                                 var urlObject = new { Url = string.Empty };
                                                                 var obj = JsonConvert.DeserializeAnonymousType(
                                                                     json, urlObject);
                                                                 return blockchain.RegisterNode(obj.Url);

                                                             //GET: http://localhost:12345/nodes/resolve
                                                             case "/nodes/resolve":
                                                                 return blockchain.Consensus();
                                                         }

                                                         return "";
                                                     }, $"http://{host}:{port}/mine/",
                                                     $"http://{host}:{port}/transactions/new/",
                                                     $"http://{host}:{port}/blockchain/",
                                                     $"http://{host}:{port}/nodes/register/",
                                                     $"http://{host}:{port}/nodes/resolve/");

            server.Run();
        }

        #endregion
    }
}