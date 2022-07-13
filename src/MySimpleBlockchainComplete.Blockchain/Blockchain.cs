#region usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using MurrayGrant.ReadablePassphrase.PhraseDescription;
using Newtonsoft.Json;

#endregion

namespace MySimpleBlockchainComplete.Blockchain;

public class Blockchain
{
    #region Constructors

    //ctor
    public Blockchain()
    {
        //var rsa = new RSACryptoServiceProvider(2048, new CspParameters());
        //string publicKey1 = Helper.MakePem(rsa.ExportSubjectPublicKeyInfo());
        //string privateKey1 = Helper.MakePem(rsa.ExportRSAPrivateKey());

        //Console.WriteLine($"Wygenerowano hasło na podstawie którego zostanie klucz prywatny {privateKey1} i publiczny {publicKey1}");

        //generowanie hasła
        //var generator = new ReadablePassphraseGenerator();
        //var defaultDict = Default.Load();
        //generator.SetDictionary(defaultDict);
        //var phrase = generator.Generate(phraseDescription: NonGrammaticalClause(10));
        //Console.WriteLine($"Wygenerowano hasło na podstawie którego zostanie klucz prywatny i publiczny {phrase}");
        var phrase = "Akron desires fiddler are halting thrived the Pakistani Havana weights optics golden";

        //generowanie klucza prywatnego
        var privateKey = Helper.GetSha256HashByteArray(phrase);
        Console.WriteLine($"Wygenerowano klucz prywatny {Helper.ConvertByteArrayToHexString(privateKey)}");

        //generowanie klucza publicznego
        var publicKey = Helper.CreatePublicKeyFromPrivate(privateKey);
        Console.WriteLine($"Wygenerowano klucz publiczny {Helper.ConvertByteArrayToHexString(publicKey)}");

        //generowanie adresu
        var address = Helper.GetSha1HashString(publicKey);
        Console.WriteLine($"Wygenerowano adres {address}");


        NodeId = Guid.NewGuid().ToString().Replace("-", "");
        CreateNewBlock(100, string.Empty);
    }

    #endregion

    #region Private properties

    private Block LastBlock
    {
        get { return blockList.Last(); }
    }

    #endregion

    #region Properties

    public string NodeId { get; }

    #endregion

    private static IEnumerable<Clause> NonGrammaticalClause(int count)
    {
        for (var i = 0; i < count; i++)
            yield return new AnyWordClause();
    }

    #region Fields

    private readonly List<Transaction> currentTransactionList = new();

    private List<Block> blockList = new();

    private readonly List<Node> nodes = new();

    #endregion

    #region Private methods

    private bool IsValidBlockList(List<Block> pBlockList)
    {
        var lastBlock = pBlockList.First();
        var currentIndex = 1;
        while (currentIndex < pBlockList.Count)
        {
            var block = pBlockList.ElementAt(currentIndex);

            if (block.PreviousHash != GetHash(lastBlock))
                return false;

            if (!IsValidNonce(lastBlock.Nonce, block.Nonce, lastBlock.PreviousHash))
                return false;

            lastBlock = block;
            currentIndex++;
        }

        return true;
    }

    private bool ResolveConflicts()
    {
        List<Block> newChain = null;

        foreach (var node in nodes)
        {
            var url = new Uri(node.Address, "/blockchain");
            var httpClient = new HttpClient();
            httpClient.BaseAddress = url;
            var response = httpClient.GetAsync(url).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var model = new
                {
                    blockList = new List<Block>(),
                    length = 0
                };
                var stream = response.Content.ReadAsStreamAsync().Result;
                var json = new StreamReader(stream).ReadToEnd();
                var data = JsonConvert.DeserializeAnonymousType(json, model);

                if (data != null && data.blockList.Count > blockList.Count && IsValidBlockList(data.blockList))
                    newChain = data.blockList;
            }
        }

        if (newChain != null)
        {
            blockList = newChain;
            return true;
        }

        return false;
    }

    private Block CreateNewBlock(int nonce, string previousHash = null)
    {
        var block = new Block(blockList.Count, DateTime.UtcNow, nonce,
            previousHash ?? GetHash(blockList.Last()),
            currentTransactionList.ToList());

        currentTransactionList.Clear();
        blockList.Add(block);
        return block;
    }

    private int FindNonce(int lastNonce, string previousHash)
    {
        var nonce = 0;
        while (!IsValidNonce(lastNonce, nonce, previousHash))
            nonce++;

        return nonce;
    }

    private bool IsValidNonce(int lastNonce, int nonce, string previousHash)
    {
        var guess = $"{lastNonce}{nonce}{previousHash}";
        var result = Helper.GetSha256HashString(guess);
        return result.StartsWith("000");
    }

    private string GetHash(Block block)
    {
        var blockText = JsonConvert.SerializeObject(block);
        return Helper.GetSha256HashString(blockText);
    }

    #endregion

    #region Public methods

    internal string Mine()
    {
        var nonce = FindNonce(LastBlock.Nonce, LastBlock.PreviousHash);

        CreateTransaction("0", NodeId, 1);
        var block = CreateNewBlock(nonce /*, _lastBlock.PreviousHash*/);

        var response = new
        {
            Message = "Nowy blok został wygenerowany",
            block.Index,
            Transactions = block.TransactionList.ToArray(),
            block.Nonce,
            block.PreviousHash
        };

        return JsonConvert.SerializeObject(response);
    }

    internal string GetFullChain()
    {
        var response = new
        {
            blockList = blockList.ToArray(),
            length = blockList.Count
        };

        return JsonConvert.SerializeObject(response);
    }

    internal string Consensus()
    {
        var replaced = ResolveConflicts();
        var message = replaced ? "został zamieniony" : "jest autorytatywny";

        var response = new
        {
            Message = $"Nasz blockchain {message}",
            BlockList = blockList
        };

        return JsonConvert.SerializeObject(response);
    }

    internal int CreateTransaction(string from, string to, double amount)
    {
        var transaction = new Transaction
        {
            From = from,
            To = to,
            Amount = amount
        };

        currentTransactionList.Add(transaction);

        return LastBlock?.Index + 1 ?? 0;
    }

    public string RegisterNode(string url)
    {
        nodes.Add(new Node
        {
            Address = new Uri($"http://{url}")
        });

        return JsonConvert.SerializeObject($"Węzeł {url} został zarejestrowany");
    }

    #endregion
}