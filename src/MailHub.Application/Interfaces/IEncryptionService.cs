﻿namespace MailHub.Application.Interfaces
{
    public interface IEncryptionService
    {
        string Decrypt(string cipherText);
        string Encrypt(string plainText);
    }
}