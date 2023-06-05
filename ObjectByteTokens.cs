using System;
using System.Text;
using System.Security;
using System.Security.Cryptography;
namespace ObjectByteConverter
{
    public class Sha1
    {
        public static string Hash(byte[] input)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(input);
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
    public enum ByteToken : byte
    {
        SHA1Check = 255,
        IdentifierName = 1,
        Int,
        Long,
        Short,
        Byte,
        Bool,
        Char,
        String,
        Ints,
        Longs,
        Shorts,
        Bytes,
        Strings,
        Compound,
    }

    public class Token
    {
        public ByteToken type { get; set; }
        public object value { get; set; }

        public static readonly Dictionary<Type, ByteToken> DataType = new Dictionary<Type, ByteToken>(){
            {typeof(int),ByteToken.Int},
            {typeof(byte),ByteToken.Byte},
            {typeof(bool),ByteToken.Bool},
            {typeof(long),ByteToken.Long},
            {typeof(char),ByteToken.Char},
            {typeof(string),ByteToken.String},

            {typeof(ICollection<int>),ByteToken.Ints},
            {typeof(ICollection<byte>),ByteToken.Bytes},
            {typeof(ICollection<long>),ByteToken.Longs},
            {typeof(ICollection<string>),ByteToken.Strings},

            {typeof(int[]),ByteToken.Ints},
            {typeof(byte[]),ByteToken.Bytes},
            {typeof(long[]),ByteToken.Longs},
            {typeof(string[]),ByteToken.Strings},

            {typeof(List<int>),ByteToken.Ints},
            {typeof(List<byte>),ByteToken.Bytes},
            {typeof(List<long>),ByteToken.Longs},
            {typeof(List<string>),ByteToken.Strings},
        };
    }
}