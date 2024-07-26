using System;
using System.IO;
using System.Security.Cryptography;

namespace CoreSample
{
    public class Program
    {
        public static byte[] Md5(byte[] message)
        {
            using (MD5 md5 = MD5.Create())
            {
                return md5.ComputeHash(message);
            }
        }

        public static byte[] Sha1(byte[] message)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                return sha1.ComputeHash(message);
            }
        }

        public static byte[] AesCbc(byte[] message)
        {
            byte[] encrypted;
            Aes aes = new AesCryptoServiceProvider();
            aes.Key = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            aes.IV = new byte[16];
            ICryptoTransform encryptor = aes.CreateEncryptor();
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(message, 0, message.Length);
                }
                encrypted = stream.ToArray();
            }

            return encrypted;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Md5");
            byte[] message = new byte[] { 0, 1, 2 };
            byte[] hash = Md5(message);
            Console.WriteLine("  Message: " + string.Join(",", message));
            Console.WriteLine("  Hash: " + string.Join(",", hash));

            Console.WriteLine("Sha1");
            message = new byte[] { 0, 1, 2 };
            hash = Sha1(message);
            Console.WriteLine("  Message: " + string.Join(",", message));
            Console.WriteLine("  Hash: " + string.Join(",", hash));

            Console.WriteLine("AesCbc");
            message = new byte[] { 0, 1, 2 };
            byte[] ciphertext = AesCbc(message);
            Console.WriteLine("  Message: " + string.Join(",", message));
            Console.WriteLine("  Ciphertext: " + string.Join(",", ciphertext));
        }
    }
}
