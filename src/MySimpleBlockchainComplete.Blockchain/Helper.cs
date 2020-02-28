#region usings

using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Math;

#endregion

namespace MySimpleBlockchainComplete.Blockchain
{
    public class Helper
    {
        public static string MakePem(byte[] ber)
        {
            StringBuilder builder = new StringBuilder();

            string base64 = Convert.ToBase64String(ber);
            int offset = 0;
            const int lineLength = 64;

            while (offset < base64.Length)
            {
                int lineEnd = Math.Min(offset + lineLength, base64.Length);
                builder.AppendLine(base64.Substring(offset, lineEnd - offset));
                offset = lineEnd;
            }

            return builder.ToString();
        }

        #region Private methods

        private static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        #endregion

        #region Public methods

        public static byte[] CreatePublicKeyFromPrivate(byte[] privateKey)
        {
            privateKey = (new byte[]
                             {
                                 0x00
                             }).Concat(privateKey).ToArray();

            var secp256K1Algorithm = SecNamedCurves.GetByName("secp256k1");
            var privateKeyInteger = new BigInteger(privateKey);

            var multiplication = secp256K1Algorithm.G.Multiply(privateKeyInteger).Normalize();
            var publicKey = new byte[65];

            var y = multiplication.AffineYCoord.ToBigInteger().ToByteArray();
            Array.Copy(y, 0, publicKey, 64 - y.Length + 1, y.Length);

            var x = multiplication.AffineXCoord.ToBigInteger().ToByteArray();
            Array.Copy(x, 0, publicKey, 32 - x.Length + 1, x.Length);

            publicKey[0] = 0x04;

            return publicKey;
        }

        public static byte[] GetSha256HashByteArray(string rawData)
        {
            using var sha256Hash = SHA256.Create();
            // ComputeHash - returns byte array  
            return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        }

        public static byte[] GetSha256HashByteArray(object obj)
        {
            var sha256 = new SHA256Managed();

            // zamiana obiektu na tablicę bajtów
            byte[] bytes = ObjectToByteArray(obj);
            // obliczanie hash-a
            return sha256.ComputeHash(bytes);
        }

        public static byte[] GetSha1HashByteArray(object obj)
        {
            var sha1 = new SHA1Managed();

            // zamiana obiektu na tablicę bajtów
            byte[] bytes = ObjectToByteArray(obj);
            // obliczanie hash-a
            return sha1.ComputeHash(bytes);
        }

        public static string GetSha256HashString(object obj)
        {
            // obliczanie hash-a
            byte[] hash = GetSha256HashByteArray(obj);

            return ConvertByteArrayToHexString(hash);
        }

        public static string GetSha1HashString(object obj)
        {
            // obliczanie hash-a
            byte[] hash = GetSha1HashByteArray(obj);

            return ConvertByteArrayToHexString(hash);
        }

        public static string ConvertByteArrayToHexString(byte[] hash)
        {
            var hashBuilder = new StringBuilder();

            // konwersja tablicy bajtów na łańcuch znaków hexadecymalnych
            foreach (byte x in hash)
                hashBuilder.Append($"{x:x2}");

            return hashBuilder.ToString();
        }

        public static IConfiguration GetConfigFromFile(string fileName)
        {
            var builder = new ConfigurationBuilder()
                          .SetBasePath(AppContext.BaseDirectory)
                          .AddJsonFile(fileName, optional: true, reloadOnChange: true);

            return builder.Build();
        }

        #endregion
    }
}