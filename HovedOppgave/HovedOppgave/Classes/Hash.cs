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
            UnicodeEncoding utf16 = new UnicodeEncoding();

            byte[] saltValue = new byte[Constants.SaltSize];
            randomGenerator.GetBytes(saltValue);
            string salt = utf16.GetString(saltValue);
            return salt;
        }

        static public Hashtable GetHashAndSalt(string password)
        {
            UnicodeEncoding utf16 = new UnicodeEncoding();
            string salt = GetSalt();
            byte[] hashValue;
            byte[] passordByte = utf16.GetBytes(password + salt);

            SHA512Managed hashString = new SHA512Managed();
            string hash = "";

            hashValue = hashString.ComputeHash(passordByte);
            foreach (byte x in hashValue)
            {
                hash += String.Format("{0:x2}", x);
            }
            Hashtable hashtable = new Hashtable();
            hashtable.Add("hash", hash);
            hashtable.Add("salt", salt);

            return hashtable;
        }

        static public string GetHash(string password, string salt)
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