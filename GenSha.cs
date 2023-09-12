



using System.Security.Cryptography;
using System.Text;

namespace ByteConverter
{
    public class GenSha
    {
        public static byte[] Sha1Hash(byte[] bytes)
        {
            byte[] hash;
            using (SHA1 sha = SHA1.Create())
            {
                hash = sha.ComputeHash(bytes);
            }
            return hash;
        }
        public static byte[] Sha256Hash(byte[] bytes)
        {
            byte[] hash;
            using (SHA256 sha = SHA256.Create())
            {
                hash = sha.ComputeHash(bytes);
            }
            return hash;
        }
        public static byte[] Sha512Hash(byte[] bytes)
        {
            byte[] hash;
            using (SHA512 sha = SHA512.Create())
            {
                hash = sha.ComputeHash(bytes);
            }
            return hash;
        }
        public static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("x2"));
            }
            return sOutput.ToString();
        }
    }
}