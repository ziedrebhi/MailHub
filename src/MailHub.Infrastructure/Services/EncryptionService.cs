using MailHub.Application.Interfaces;
using System.Security.Cryptography;

namespace MailHub.Infrastructure.Services
{
    public class EncryptionService : IEncryptionService
    {
        // Use a securely generated 256-bit key
        private readonly string _encryptionKey = "B5A6F2E313A9C19D4D4E8B6AB5285AC1A6B9F0C863FD9827D1D7B05F8E3EFC4F"; // 256-bit key (32 bytes)

        public string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = HexStringToByteArray(_encryptionKey);  // Convert the hex string to byte array
                aesAlg.IV = new byte[16]; // Use 16 bytes of 0 for the IV (or generate a random IV for better security)

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray()); // Return the encrypted text as base64 string
                    }
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = HexStringToByteArray(_encryptionKey);  // Convert the hex string to byte array
                aesAlg.IV = new byte[16]; // Use the same IV used during encryption

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd(); // Return the decrypted text
                        }
                    }
                }
            }
        }

        // Helper function to convert hex string to byte array
        private byte[] HexStringToByteArray(string hex)
        {
            int length = hex.Length;
            byte[] byteArray = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                byteArray[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return byteArray;
        }
    }
}
