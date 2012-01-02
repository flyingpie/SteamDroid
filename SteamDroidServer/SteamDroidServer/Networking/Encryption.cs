using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using SteamDroidServer.Logging;

namespace SteamDroidServer.Networking
{
    /// <summary>
    /// Encryption, encrypts and decrypts data from- and to base64 encoded AES
    /// </summary>
    public class Encryption
    {
        public const String KeyFile = "aes.txt";

        private const int KeySize = 16;

        private static byte[] key;

        public static void SetKey(String key)
        {
            byte[] keyBytes = new UTF8Encoding().GetBytes(key);
            String hash = Convert.ToBase64String(SHA1.Create().ComputeHash(keyBytes));
            Encryption.key = new byte[KeySize];

            for (int i = 0; i < KeySize; i++)
            {
                Encryption.key[i] = (byte)hash[i];
            }
        }

        /// <summary>
        /// Encrypts the specified data, using the specified algorithm
        /// </summary>
        /// <typeparam name="TSymmetricAlgorithm"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Encrypt<TSymmetricAlgorithm>(string input) where TSymmetricAlgorithm : SymmetricAlgorithm, new()
        {
            //var pwdBytes = Encoding.UTF8.GetBytes(key);
            using (TSymmetricAlgorithm sa = new TSymmetricAlgorithm())
            {
                ICryptoTransform saEnc = sa.CreateEncryptor(key, key);

                var encBytes = Encoding.UTF8.GetBytes(input);

                var resultBytes = saEnc.TransformFinalBlock(encBytes, 0, encBytes.Length);

                return Convert.ToBase64String(resultBytes);
            }
        }

        /// <summary>
        /// Decrypts the specified data, using the specified algorithm
        /// </summary>
        /// <typeparam name="TSymmetricAlgorithm"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decrypt<TSymmetricAlgorithm>(string input) where TSymmetricAlgorithm : SymmetricAlgorithm, new()
        {
            //var pwdBytes = Encoding.UTF8.GetBytes(key);
            using (TSymmetricAlgorithm sa = new TSymmetricAlgorithm())
            {
                ICryptoTransform saDec = sa.CreateDecryptor(key, key);

                var encBytes = Convert.FromBase64String(input);

                var resultBytes = saDec.TransformFinalBlock(encBytes, 0, encBytes.Length);
                return Encoding.UTF8.GetString(resultBytes);
            }
        }

    }
}
