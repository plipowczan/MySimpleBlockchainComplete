#region usings

using System;

#endregion

namespace MySimpleBlockchainWithPoW.Blockchain
{
    [Serializable]
    public class Transaction
    {
        #region Properties

        public double Amount { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        #endregion
    }
}