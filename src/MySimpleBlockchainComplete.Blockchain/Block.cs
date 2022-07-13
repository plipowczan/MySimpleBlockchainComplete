#region usings

using System;
using System.Collections.Generic;

#endregion

namespace MySimpleBlockchainComplete.Blockchain;

[Serializable]
public class Block
{
    #region Constructors

    public Block(int index, DateTime timestamp, int nonce, string previousHash, List<Transaction> transactionList)
    {
        Index = index;
        Timestamp = timestamp;
        Nonce = nonce;
        PreviousHash = previousHash;
        TransactionList = transactionList;
    }

    #endregion

    #region Public methods

    public override string ToString()
    {
        return
            $"{Index} [{Timestamp:yyyy-MM-dd HH:mm:ss}] Nonce: {Nonce} | PrevHash: {PreviousHash} | TrCnt: {TransactionList.Count}";
    }

    #endregion

    #region Properties

    public int Index { get; set; }

    public DateTime Timestamp { get; set; }

    public int Nonce { get; set; }

    public string PreviousHash { get; set; }

    public List<Transaction> TransactionList { get; set; }

    #endregion
}