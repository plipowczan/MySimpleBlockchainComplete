#region usings

using System;
using System.Collections.Generic;

#endregion

namespace MySimpleBlockchainComplete.Blockchain
{
    [Serializable]
    public class Block
    {
        #region Constructors

        public Block(int index, DateTime timestamp, int nonce, string previousHash, List<Transaction> transactionList)
        {
            this.Index = index;
            this.Timestamp = timestamp;
            this.Nonce = nonce;
            this.PreviousHash = previousHash;
            this.TransactionList = transactionList;
        }

        #endregion

        #region Properties

        public int Index { get; set; }

        public DateTime Timestamp { get; set; }

        public int Nonce { get; set; }

        public string PreviousHash { get; set; }

        public List<Transaction> TransactionList { get; set; }

        #endregion

        #region Public methods

        public override string ToString()
        {
            return
                $"{this.Index} [{this.Timestamp:yyyy-MM-dd HH:mm:ss}] Nonce: {this.Nonce} | PrevHash: {this.PreviousHash} | TrCnt: {this.TransactionList.Count}";
        }

        #endregion
    }
}