using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ObjectByteConverter
{
    public class ObjectSerializer
    {
        public const string ClassName = "$CN";
        public const string DatagramLength = "$DL";
        public const string ShaVerified = "$SV";
        public const string LengthReader = "$LR";
        public const string ShaVerificationCode = "$SVC";
        public List<byte> Buffer { get; set; }
        public bool VerifyWhenSerializing { get; set; }
        public delegate void LengthWriter(int length);
        public LengthWriter WriteLength;
        public ObjectSerializer(bool verifyWhenSerializing = true)
        {
            Buffer = new List<byte>();
            VerifyWhenSerializing = verifyWhenSerializing;
            WriteLength = (int length) =>
            {
                if (length >= ushort.MinValue && length <= ushort.MaxValue)
                {
                    WriteLiteral((ushort)length);
                }
                else
                {
                    throw new Exception($"Length cannot be less than 0 or more than {ushort.MaxValue}");
                }
            };
        }
        public byte[] SerializeProperties(object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            string className = obj.GetType().ToString();
            Dictionary<string, object> header = new()
            {
                { DatagramLength, 0 },
                {ShaVerified,VerifyWhenSerializing}
            };
            Dictionary<string, object> data = new(){
                {ClassName,className}
            };

            foreach (var property in properties)
            {
                string name = property.Name;
                var type_ = Token.DataType[property.PropertyType];
                object val = property.GetValue(obj) ?? fromNullToVal(type_);
                data.Add(name, val);
            }

            if (VerifyWhenSerializing)
            {
                byte[] hash = GenSha.Sha1Hash(DictToBytes(data));
                header.Add(ShaVerificationCode,hash);
            }
            foreach(var v in data.Keys){
                header.Add(v,data[v]);
            }
            int bts = DictToBytes(header).Length;
            Console.WriteLine("Length of datagram : {0}",bts);
            header[DatagramLength] = bts;

            foreach(var d in header.Keys){
                Console.Write($"[{d}][{header[d]}]");
            }
            Console.WriteLine();
            return DictToBytes(header);
        }
        private byte[] DictToBytes(Dictionary<string, object> values)
        {
            Buffer = new List<byte>();
            foreach (var var in values.Keys)
            {
                WriteLiteral((byte)ByteToken.IdentifierName);
                WriteASCII(var);
                var type_ = Token.DataType[values[var].GetType()];
                WriteLiteral((byte)Token.DataType[values[var].GetType()]);
                Write(values[var], type_);
            }
            return Buffer.ToArray();
        }
        void Write(object obj, ByteToken type)
        {
            if (obj is int)
            {
                WriteLiteral((int)obj);
            }
            else if (obj is uint)
            {
                WriteLiteral((uint)obj);
            }
            else if (obj is short)
            {
                WriteLiteral((short)obj);
            }
            else if (obj is ushort)
            {
                WriteLength((ushort)obj);
            }
            else if (obj is byte)
            {
                WriteLiteral((byte)obj);
            }
            else if (obj is long)
            {
                WriteLiteral((long)obj);
            }
            else if (obj is ulong)
            {
                WriteLiteral((ulong)obj);
            }
            else if (obj is float)
            {
                WriteLiteral((float)obj);
            }
            else if (obj is double)
            {
                WriteLiteral((double)obj);
            }
            else if (obj is DateTime)
            {
                WriteLiteral((DateTime)obj);
            }
            else if (obj is char)
            {
                WriteLiteral((char)obj);
            }
            else if (obj is bool)
            {
                WriteLiteral((bool)obj);
            }
            else if (obj is string)
            {
                WriteLiteral((string)obj);
            }
            else if (obj is ICollection<int>)
            {
                WriteLiteral((ICollection<int>)obj);
            }
            else if (obj is ICollection<short>)
            {
                WriteLiteral((ICollection<short>)obj);
            }
            else if (obj is ICollection<long>)
            {
                WriteLiteral((ICollection<long>)obj);
            }
            else if (obj is ICollection<uint>)
            {
                WriteLiteral((ICollection<uint>)obj);
            }
            else if (obj is ICollection<ushort>)
            {
                WriteLiteral((ICollection<ushort>)obj);
            }
            else if (obj is ICollection<ulong>)
            {
                WriteLiteral((ICollection<ulong>)obj);
            }
            else if (obj is ICollection<byte>)
            {
                WriteLiteral((ICollection<byte>)obj);
            }
            else if (obj is ICollection<string>)
            {
                WriteLiteral((ICollection<string>)obj);
            }
            else if (obj is ICollection<float>)
            {
                WriteLiteral((ICollection<float>)obj);
            }
            else if (obj is ICollection<double>)
            {
                WriteLiteral((ICollection<double>)obj);
            }
            else if (obj is Guid)
            {
                WriteLiteral((Guid)obj);
            }
            else if (obj is Vector3)
            {
                WriteLiteral((Vector3)obj);
            }
            else if (obj is Quaternion)
            {
                WriteLiteral((Quaternion)obj);
            }
            else if (obj is null)
            {
                WriteNull(type);
            }
            else
            {
                throw new Exception($"Unsupported type : {obj.GetType()}");
            }
        }
        object fromNullToVal(ByteToken type)
        {
            return type switch
            {
                ByteToken.Int => (int)0,
                ByteToken.UInt => (uint)0,
                ByteToken.Byte => (byte)0,
                ByteToken.Short => (short)0,
                ByteToken.UShort => (ushort)0,
                ByteToken.DateTime => new DateTime(0),
                ByteToken.Long => (long)0,
                ByteToken.ULong => (ulong)0,
                ByteToken.Float => (float)0,
                ByteToken.Double => (double)0,
                ByteToken.Vector3 => Vector3.zero,
                ByteToken.Quaternion => Quaternion.identity,
                ByteToken.Char => (char)0,
                ByteToken.Bool => false,
                ByteToken.String => "",
                ByteToken.GUID => Guid.Empty,
                ByteToken.Ints => new int[] { },
                ByteToken.UInts => new uint[] { },
                ByteToken.Bytes => new byte[] { },
                ByteToken.Shorts => new short[] { },
                ByteToken.Longs => new long[] { },
                ByteToken.UShorts => new ushort[] { },
                ByteToken.ULongs => new ulong[] { },
                ByteToken.Floats => new float[] { },
                ByteToken.Doubles => new double[] { },
                ByteToken.Strings => new string[] { },
                _ => throw new Exception($"Illegal type : {type}"),
            };
        }
        void WriteNull(ByteToken type)
        {
            switch (type)
            {
                case ByteToken.Int:
                    WriteLiteral((int)0);
                    break;
                case ByteToken.UInt:
                    WriteLiteral((uint)0);
                    break;
                case ByteToken.Byte:
                    WriteLiteral((byte)0);
                    break;
                case ByteToken.Short:
                    WriteLiteral((short)0);
                    break;
                case ByteToken.UShort:
                    WriteLiteral((ushort)0);
                    break;
                case ByteToken.DateTime:
                    WriteLiteral(new DateTime(0));
                    break;
                case ByteToken.Long:
                    WriteLiteral((long)0);
                    break;
                case ByteToken.ULong:
                    WriteLiteral((ulong)0);
                    break;
                case ByteToken.Float:
                    WriteLiteral((float)0);
                    break;
                case ByteToken.Double:
                    WriteLiteral((double)0);
                    break;
                case ByteToken.Vector3:
                    WriteLiteral(Vector3.zero);
                    break;
                case ByteToken.Quaternion:
                    WriteLiteral(Quaternion.identity);
                    break;
                case ByteToken.Char:
                    WriteLiteral((char)0);
                    break;
                case ByteToken.Bool:
                    WriteLiteral(false);
                    break;
                case ByteToken.String:
                    WriteLiteral("");
                    break;
                case ByteToken.GUID:
                    WriteLiteral(Guid.Empty);
                    break;
                case ByteToken.Ints:
                    WriteLiteral(new int[] { });
                    break;
                case ByteToken.UInts:
                    WriteLiteral(new uint[] { });
                    break;
                case ByteToken.Bytes:
                    WriteLiteral(new byte[] { });
                    break;
                case ByteToken.Shorts:
                    WriteLiteral(new short[] { });
                    break;
                case ByteToken.Longs:
                    WriteLiteral(new long[] { });
                    break;
                case ByteToken.UShorts:
                    WriteLiteral(new ushort[] { });
                    break;
                case ByteToken.ULongs:
                    WriteLiteral(new ulong[] { });
                    break;
                case ByteToken.Floats:
                    WriteLiteral(new float[] { });
                    break;
                case ByteToken.Doubles:
                    WriteLiteral(new double[] { });
                    break;
                case ByteToken.Strings:
                    WriteLiteral(new string[] { });
                    break;

                default:
                    throw new Exception($"Illegal type : {type.ToString()}");
            }
        }

        void WriteLiteral(int val)
        {
            Buffer.AddRange(BitConverter.GetBytes(val));
        }
        void WriteLiteral(long val)
        {
            Buffer.AddRange(BitConverter.GetBytes(val));
        }
        void WriteLiteral(short val)
        {
            Buffer.AddRange(BitConverter.GetBytes(val));
        }
        void WriteLiteral(uint val)
        {
            Buffer.AddRange(BitConverter.GetBytes(val));
        }
        void WriteLiteral(ulong val)
        {
            Buffer.AddRange(BitConverter.GetBytes(val));
        }
        void WriteLiteral(ushort val)
        {
            Buffer.AddRange(BitConverter.GetBytes(val));
        }
        void WriteLiteral(byte val)
        {
            Buffer.Add(val);
        }
        void WriteLiteral(float val)
        {
            Buffer.AddRange(BitConverter.GetBytes(val));
        }
        void WriteLiteral(double val)
        {
            Buffer.AddRange(BitConverter.GetBytes(val));
        }
        void WriteLiteral(DateTime val)
        {
            Buffer.AddRange(BitConverter.GetBytes(val.ToBinary()));
        }
        void WriteLiteral(char val)
        {
            Buffer.AddRange(BitConverter.GetBytes(val));
        }
        void WriteLiteral(bool val)
        {
            Buffer.AddRange(BitConverter.GetBytes(val));
        }
        void WriteLiteral(string val)
        {
            WriteLength(val.Length);
            Buffer.AddRange(Encoding.Unicode.GetBytes(val));
        }
        void WriteASCII(string val)
        {
            WriteLength(val.Length);
            Buffer.AddRange(Encoding.ASCII.GetBytes(val));
        }
        void WriteLiteral(Vector3 val)
        {
            WriteLiteral(val.x);
            WriteLiteral(val.y);
            WriteLiteral(val.z);
        }
        void WriteLiteral(Quaternion val)
        {
            WriteLiteral(val.x);
            WriteLiteral(val.y);
            WriteLiteral(val.z);
            WriteLiteral(val.w);
        }
        void WriteLiteral(Guid val)
        {
            WriteLiteral(val.ToByteArray());
        }
        void WriteLiteral(ICollection<int> val)
        {
            WriteLength(val.Count());
            foreach (var item in val)
            {
                WriteLiteral(item);
            }
        }
        void WriteLiteral(ICollection<long> val)
        {
            WriteLength(val.Count());
            foreach (var item in val)
            {
                WriteLiteral(item);
            }
        }
        void WriteLiteral(ICollection<short> val)
        {
            WriteLength(val.Count());
            foreach (var item in val)
            {
                WriteLiteral(item);
            }
        }
        void WriteLiteral(ICollection<uint> val)
        {
            WriteLength(val.Count());
            foreach (var item in val)
            {
                WriteLiteral(item);
            }
        }
        void WriteLiteral(ICollection<ulong> val)
        {
            WriteLength(val.Count());
            foreach (var item in val)
            {
                WriteLiteral(item);
            }
        }
        void WriteLiteral(ICollection<ushort> val)
        {
            WriteLength(val.Count());
            foreach (var item in val)
            {
                WriteLiteral(item);
            }
        }
        void WriteLiteral(ICollection<float> val)
        {
            WriteLength(val.Count());
            foreach (var item in val)
            {
                WriteLiteral(item);
            }
        }
        void WriteLiteral(ICollection<double> val)
        {
            WriteLength(val.Count());
            foreach (var item in val)
            {
                WriteLiteral(item);
            }
        }
        void WriteLiteral(ICollection<byte> val)
        {
            WriteLength(val.Count());
            foreach (var item in val)
            {
                WriteLiteral(item);
            }
        }
        void WriteLiteral(ICollection<string> val)
        {
            WriteLength(val.Count());
            foreach (var item in val)
            {
                WriteLiteral(item);
            }
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
