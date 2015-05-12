using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace HovedOppgave.Classes
{
    /// <summary>
    /// Salter og hasher passordet og retunere stringen tilbake
    /// 
    /// Gjeste forfatter: Eivind Skreddernes
    /// </summary>
    
    public class Hash
    {
        private static RNGCryptoServiceProvider randomGenerator = new RNGCryptoServiceProvider();

        static public string GetSalt()
        {
            // Create an array of random values.
            byte[] saltValue = new byte[Constant.SaltSize];
            randomGenerator.GetBytes(saltValue);
            string salt = Convert.ToBase64String(saltValue);
            return salt;
        }

        static public Hashtable GetHashAndSalt(string password)
        {
            string salt = GetSalt();
            string hash = CreateHash(password, salt);

            Hashtable hashtable = new Hashtable();
            hashtable.Add("hash", hash);
            hashtable.Add("salt", salt);

            return hashtable;
        }

        static public string GetHash(string password, string salt)
        {
            string hash = CreateHash(password, salt);
            return hash;
        }

        static private string CreateHash(string password, string salt)
        {
            UnicodeEncoding utf16 = new UnicodeEncoding();
            byte[] hashValue;
            byte[] passwordByte = utf16.GetBytes(password + salt);

            SHA512Managed hashString = new SHA512Managed();
            string hash = "";

            hashValue = hashString.ComputeHash(passwordByte);
            foreach (byte x in hashValue)
            {
                hash += String.Format("{0:x2}", x);
            }
            return hash;
        }

        static public bool CheckPassword(string password, string hash, string salt)
        {
            string hash2 = GetHash(password, salt);
            return hash.Equals(hash2);
        }
    }
}