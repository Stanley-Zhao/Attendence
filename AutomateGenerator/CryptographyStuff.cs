using System;
using System.Net;
using System.Windows;

using System.Security.Cryptography;
using System.IO;
using System.Text;
namespace AutomateGenerator
{
    public class CryptographyStuff
    {
        //Never hard code a password within your source code. 
        //Hard-coded passwords can be retrieved from an assembly using the MSIL Disassembler (Ildasm.exe), a hex editor, 
        //or by simply opening up the assembly in a text editor such as Notepad.exe. 
        static readonly string password = "MyPassword.1";
        static readonly byte[] salt = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xF1, 0xF0, 0xEE, 0x21, 0x22, 0x45 };

        public static string AES_EncryptString(string plaintext)
        {
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt);
            AesManaged aes = new AesManaged();
            aes.Key = key.GetBytes(32);
            aes.IV = key.GetBytes(16);

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(cs);
            sw.WriteLine(plaintext);
            sw.Close();
            cs.Close();
            byte[] buffer = ms.ToArray();
            string encryptedString = BytesToHexString(buffer);
            ms.Close();
            key.Reset();
            return encryptedString;
        }

        public static string AES_DecryptString(string encryptedtext)
        {
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt);
            AesManaged aes = new AesManaged();
            aes.Key = key.GetBytes(32);
            aes.IV = key.GetBytes(16);

            byte[] buffer = HexStringToBytes(encryptedtext);
            MemoryStream ms = new MemoryStream(buffer);
            CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            string rawString = sr.ReadLine();
            sr.Close();
            cs.Close();
            ms.Close();
            key.Reset();
            return rawString;
        }

        /// <summary>
        /// Convert a byte array to a hex-encoded string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        static string BytesToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        /// <summary>
        /// Convert a hex-encoded string to a byte array
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        static byte[] HexStringToBytes(string hexString)
        {
            int numChars = hexString.Length;
            byte[] bytes = new byte[numChars / 2];
            for (int i = 0; i < numChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            return bytes;
        }
    }
}
