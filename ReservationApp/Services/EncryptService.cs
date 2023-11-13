using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using ReservationApp.Services.Interface;
using System.Security.Cryptography;
using System.Text;

namespace ReservationApp.Services
{
    public class EncryptService : IEncryptService
    {
        public Encoding EncodingType { get; set; } = Encoding.UTF8;

        public string Key
        {
            get;
            set;
        } = "1234567812345678";

        public string Iv
        {
            get;
            set;
        } = "8765432187654321";

        public string Encrypt(string data)
        {
            return Encrypt(data, Key, Iv);
        }

        public string Encrypt(string data, byte[] key, byte[] iv)
        {
            if(string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            byte[] encrypted;
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using(MemoryStream msEncyrpt = new MemoryStream())
                {
                    using(CryptoStream csEncrypt = new CryptoStream(msEncyrpt, encryptor, CryptoStreamMode.Write))
                    {
                        using(StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(data);
                        }
                        encrypted = msEncyrpt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
            //return EncodingType.GetString(encrypted);
        }
        public string Encrypt(string data, string key , string iv)
        {
            byte[] key_bytes = EncodingType.GetBytes(key);
            byte[] iv_bytes = EncodingType.GetBytes(iv);
            return Encrypt(data, key_bytes, iv_bytes);
        }

        public string Decrypt(string data)
        {
            return Decrypt(data, Key, Iv);
        }

        public string Decrypt(string data, string key, string iv)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            byte[] data_bytes = Convert.FromBase64String(data);
            byte[] key_bytes = EncodingType.GetBytes(key);
            byte[] iv_bytes = EncodingType.GetBytes(iv);
            return Decrypt(data_bytes, key_bytes, iv_bytes);
        }

        public string Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            string decrypted = null;
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
               
                using (MemoryStream msDecyrpt = new MemoryStream(data))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecyrpt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            decrypted = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return decrypted;
        }
    }
}
