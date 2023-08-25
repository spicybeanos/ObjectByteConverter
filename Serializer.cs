using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ByteConverter
{
    public class Serializer
    {
        public const string ClassName = "$CN";
        public const string DatagramLength = "$DL";
        public const string ShaVerified = "$SV";
        public const string ShaVerificationCode = "$VC";
        public ByteToken LengthSize { get; private set; }
        public delegate byte[] LWriter(object data);
        public delegate byte[] SWriter(string data);
        public LWriter LengthWriter { get;private set; }
        public SWriter StringWriter { get;private set; }
        public LWriter MetaInfLengthWriter { get;private set; }
        
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
            StringWriter = WriteString_Unicode;
        }
        public Serializer(ByteToken lengthSize, LWriter lengthWriter)
        {
            LengthSize = lengthSize;
            LengthWriter = lengthWriter;
            MetaInfLengthWriter = (object data) =>
            {
                return new byte[] { (byte)(int)data };
            };
            StringWriter = WriteString_Unicode;
        }
        public Serializer(SWriter stringWriter)
        {
            StringWriter = stringWriter;
        }
        public Serializer(ByteToken lengthSize, LWriter lengthWriter,SWriter stringWriter)
        {
            LengthSize = lengthSize;
            LengthWriter = lengthWriter;
            MetaInfLengthWriter = (object data) =>
            {
                return new byte[] { (byte)(int)data };
            };
            StringWriter = stringWriter;
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

        byte[] WriteLiteral(object val)
        {
            ByteToken dt = Token.DataType[val.GetType()];
            return dt switch
            {
                ByteToken.Byte => new byte[] { (byte)val },
                ByteToken.SByte => BitConverter.GetBytes((sbyte)val),
                ByteToken.Short => BitConverter.GetBytes((short)val),
                ByteToken.UShort => BitConverter.GetBytes((ushort)val),
                ByteToken.Int => BitConverter.GetBytes((int)val),
                ByteToken.UInt => BitConverter.GetBytes((uint)val),
                ByteToken.Long => BitConverter.GetBytes((long)val),
                ByteToken.ULong => BitConverter.GetBytes((ulong)val),
                ByteToken.Float => BitConverter.GetBytes((float)val),
                ByteToken.Double => BitConverter.GetBytes((double)val),
                ByteToken.Char => BitConverter.GetBytes((char)val),
                ByteToken.Bool => BitConverter.GetBytes((bool)val),
                ByteToken.String => StringWriter((string)val),
                _ => throw new Exception($"Type not supported : {dt}")
            };
        }

        public byte[] WriteString_Unicode(string val)
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

        byte[] WriteLiteralArray<T>(ICollection<T> val)
        {
            List<byte> r = new();
            r.AddRange(LengthWriter(val.Count));
            foreach (var v in val)
            {
                r.AddRange(WriteLiteral(v));
            }
            return r.ToArray();
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
