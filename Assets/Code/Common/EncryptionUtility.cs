using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionUtility
{
    private static string encryptionKey = "YourStrongKeyHere123";

    public static string EncryptString(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
            aes.GenerateIV();
            byte[] ivBytes = aes.IV;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(ivBytes, 0, ivBytes.Length);

                using (CryptoStream cryptoStream = new CryptoStream(
                    memoryStream,
                    aes.CreateEncryptor(),
                    CryptoStreamMode.Write
                ))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    cryptoStream.FlushFinalBlock();

                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }
    }

    public static string DecryptString(string encryptedText)
    {
        byte[] allBytes = Convert.FromBase64String(encryptedText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(encryptionKey);

            byte[] ivBytes = new byte[aes.BlockSize / 8];
            Array.Copy(allBytes, 0, ivBytes, 0, ivBytes.Length);
            aes.IV = ivBytes;

            int cipherStart = ivBytes.Length;
            int cipherLength = allBytes.Length - cipherStart;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(
                    memoryStream,
                    aes.CreateDecryptor(),
                    CryptoStreamMode.Write
                ))
                {
                    cryptoStream.Write(allBytes, cipherStart, cipherLength);
                    cryptoStream.FlushFinalBlock();

                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
        }
    }
}
