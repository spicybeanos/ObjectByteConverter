using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ObjectByteConverter
{
    public class Serializer
    {
        public const string ClassName = "$CN";
        public const string DatagramLength = "$DL";
        public const string ShaVerified = "$SV";
        public const string ShaVerificationCode = "$VC";
        public ByteToken LengthSize { get; private set; }
        public delegate byte[] Writer(object data);
        public Writer LengthWriter;
        public Writer MetaInfLengthWriter;
        public Serializer()
        {
            LengthWriter = (object data) =>
            {
                LengthSize = ByteToken.UShort;
                return BitConverter.GetBytes((ushort)(int)data);
            };
            MetaInfLengthWriter = (object data) =>
            {
                return new byte[] { (byte)(int)data };
            };
        }
        public Serializer(ByteToken lengthSize, Writer lengthWriter)
        {
            LengthSize = lengthSize;
            LengthWriter = lengthWriter;
        }

        public byte[] Meta_WriteString(string val)
        {
            List<byte> r = new List<byte>();
            r.AddRange(MetaInfLengthWriter(val.Length));
            r.AddRange(Encoding.UTF8.GetBytes(val));
            return r.ToArray();
        }
        public byte[] Meta_WriteByteArray(ICollection<byte> val)
        {
            List<byte> r = new List<byte>();
            r.AddRange(MetaInfLengthWriter(val.Count));
            r.AddRange(val);
            return r.ToArray();
        }

        byte[] WriteLiteral(byte val)
        {
            return new byte[] { val };
        }
        byte[] WriteLiteral(sbyte val)
        {
            return BitConverter.GetBytes(val);
        }
        byte[] WriteLiteral(short val)
        {
            return BitConverter.GetBytes(val);
        }
        byte[] WriteLiteral(ushort val)
        {
            return BitConverter.GetBytes(val);
        }
        byte[] WriteLiteral(int val)
        {
            return BitConverter.GetBytes(val);
        }
        byte[] WriteLiteral(uint val)
        {
            return BitConverter.GetBytes(val);
        }
        byte[] WriteLiteral(long val)
        {
            return BitConverter.GetBytes(val);
        }
        byte[] WriteLiteral(ulong val)
        {
            return BitConverter.GetBytes(val);
        }
        byte[] WriteLiteral(float val)
        {
            return BitConverter.GetBytes(val);
        }
        byte[] WriteLiteral(double val)
        {
            return BitConverter.GetBytes(val);
        }
        public byte[] WriteString(string val)
        {
            List<byte> r = new List<byte>();
            r.AddRange(LengthWriter(val.Length));
            r.AddRange(Encoding.Unicode.GetBytes(val));
            return r.ToArray();
        }
        public byte[] WriteString_UTF8(string val)
        {
            List<byte> r = new List<byte>();
            r.AddRange(LengthWriter(val.Length));
            r.AddRange(Encoding.UTF8.GetBytes(val));
            return r.ToArray();
        }

        byte[] WriteLiteral(IEnumerable<byte> val)
        {
            List<byte> r= new();
            r.AddRange(LengthWriter(val.Count));
            r.AddRange(val);
            return r.ToArray();
        }
        byte[] WriteLiteral(ICollection<sbyte> val)
        {
            List<byte> r= new();
            r.AddRange(LengthWriter(val));
            for (int i = 0; i < val.Count; i++)
            {
                r.AddRange(WriteLiteral(val[i]));
            }
            return r.ToArray();
        }
        byte[] WriteLiteral(short val)
        {
            return BitConverter.GetBytes(val);
        }
        byte[] WriteLiteral(ushort val)
        {
            return BitConverter.GetBytes(val);
        }
        byte[] WriteLiteral(int val)
        {
            return BitConverter.GetBytes(val);
        }
        byte[] WriteLiteral(uint val)
        {
            return BitConverter.GetBytes(val);
        }
        byte[] WriteLiteral(long val)
        {
            return BitConverter.GetBytes(val);
        }
        byte[] WriteLiteral(ulong val)
        {
            return BitConverter.GetBytes(val);
        }
        byte[] WriteLiteral(float val)
        {
            return BitConverter.GetBytes(val);
        }
        byte[] WriteLiteral(double val)
        {
            return BitConverter.GetBytes(val);
        }

    }

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
