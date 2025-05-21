using KGTT_Educate.Services.Account.Models;
using System.Security.Cryptography;
using System.Text;

namespace KGTT_Educate.Services.Account.Utils
{
    public class Hasher
    {
        public string HashSHA256(string data)
        {
            SHA256 sha256 = SHA256.Create();

            byte[] dataBytes = Encoding.ASCII.GetBytes(data);

            byte[] hash = sha256.ComputeHash(dataBytes);

            return Convert.ToHexString(hash);
        }

        public string HashSHA512(string data)
        {
            SHA512 sha512 = SHA512.Create();

            byte[] dataBytes = Encoding.ASCII.GetBytes(data);

            byte[] hash = sha512.ComputeHash(dataBytes);

            return Convert.ToHexString(hash);
        }

        public bool CompareWithSHA256(string unhashedData, string hashedData)
        {
            SHA256 sHA256 = SHA256.Create();

            byte[] unhashedDataBytes = Encoding.ASCII.GetBytes(unhashedData);

            if (Convert.ToHexString(sHA256.ComputeHash(unhashedDataBytes)) == hashedData)
            {
                return true;
            }

            return false;
        }

        public bool CompareWithSHA512(string unhashedData, string hashedData)
        {
            SHA512 sHA512 = SHA512.Create();

            byte[] unhashedDataBytes = Encoding.ASCII.GetBytes(unhashedData);

            if (Convert.ToHexString(sHA512.ComputeHash(unhashedDataBytes)) == hashedData)
            {
                return true;
            }

            return false;
        }
    }
}
