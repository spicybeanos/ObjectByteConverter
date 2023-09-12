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
        public const string LengthReader = "$LR";
        public const string CustomTypeCount = "$CT";
        public const string EndOfMeta = "&0M";
        public const string StructName = "%SN";
        public const string VariableCount = "%VC";
        public ByteToken LengthSize { get; private set; }
        public delegate byte[] LWriter(object data);
        public delegate byte[] SWriter(string data);
        public LWriter LengthWriter { get; private set; }
        public SWriter StringWriter { get; private set; }
        public LWriter MetaInfLengthWriter { get; private set; }

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
            MetaInfLengthWriter = (object data) =>
            {
                return new byte[] { (byte)(int)data };
            };
        }
        public Serializer(ByteToken lengthSize, LWriter lengthWriter, SWriter stringWriter)
        {
            LengthSize = lengthSize;
            LengthWriter = lengthWriter;
            MetaInfLengthWriter = (object data) =>
            {
                return new byte[] { (byte)(int)data };
            };
            StringWriter = stringWriter;
        }

        private byte[] Meta_WriteString(string val)
        {
            List<byte> r = new List<byte>();
            r.AddRange(MetaInfLengthWriter(val.Length));
            r.AddRange(Encoding.UTF8.GetBytes(val));
            return r.ToArray();
        }
        private byte[] Meta_WriteByteArray(ICollection<byte> val)
        {
            List<byte> r = new List<byte>();
            if (val != null)
            {
                r.AddRange(MetaInfLengthWriter(val.Count));
                r.AddRange(val);
            }
            else
            {
                r.AddRange(MetaInfLengthWriter(0));
                r.AddRange(new byte[0] { });
            }
            return r.ToArray();
        }
        private byte[] TypeDef_WriteCustomDefinition(Type type)
        {
            List<byte> r = new();
            r.AddRange(Meta_WriteProperty(StructName, type.ToString()));
            uint ctr = 0;
            var props = type.GetProperties();
            foreach (var p in props)
            {
                if (p.PropertyType.IsPublic && p.PropertyType.IsPrimitive)
                {
                    ctr++;
                }
            }
            r.AddRange(Meta_WriteProperty(VariableCount, ctr));

            return r.ToArray();
        }
        private byte[] Meta_WriteProperty(string name, object val)
        {
            List<byte> r = new()
            {
                (byte)ByteToken.Meta_Identifer
            };
            r.AddRange(Meta_WriteString(name));
            if (val is string)
            {
                r.Add((byte)ByteToken.Meta_String);
                r.AddRange(Meta_WriteString((string)val));
            }
            else if (val is ICollection<byte>)
            {
                r.Add((byte)ByteToken.Bytes);
                r.AddRange(Meta_WriteByteArray((byte[])val));
            }
            else if (val is ByteToken)
            {
                r.Add((byte)ByteToken.Byte);
                r.Add((byte)(ByteToken)val);
            }
            else
            {
                r.Add((byte)Token.GetByteToke(val.GetType()));
                r.AddRange(WriteLiteral(val));
            }
            return r.ToArray();
        }

        private byte[] GenerateMetaInf(string className, uint datagramLength,
         bool shaVerify, uint customTypes, byte[] shaCode)
        {
            List<byte> r = new();
            r.AddRange(Meta_WriteProperty(ClassName, className));
            r.AddRange(Meta_WriteProperty(DatagramLength, datagramLength));
            r.AddRange(Meta_WriteProperty(ShaVerified, shaVerify));
            r.AddRange(Meta_WriteProperty(CustomTypeCount, customTypes));
            r.AddRange(Meta_WriteProperty(LengthReader, LengthSize));
            r.AddRange(Meta_WriteProperty(ShaVerificationCode, shaCode));

            return r.ToArray();
        }

        private byte[] WriteString_Unicode(string val)
        {
            List<byte> r = new List<byte>();
            r.AddRange(LengthWriter(val.Length));
            r.AddRange(Encoding.Unicode.GetBytes(val));
            return r.ToArray();
        }
        private byte[] WriteString_UTF8(string val)
        {
            List<byte> r = new List<byte>();

            r.AddRange(LengthWriter(val.Length));
            r.AddRange(Encoding.UTF8.GetBytes(val));
            return r.ToArray();
        }
        private byte[] WriteProperty(PropertyInfo prop, object obj)
        {
            List<byte> ret = new()
            {
                (byte)ByteToken.IdentifierName
            };
            ret.AddRange(WriteString_UTF8(prop.Name));
            ret.AddRange(Write(prop.GetValue(obj)));
            return ret.ToArray();
        }

        protected byte[] WriteCustomType(object obj)
        {
            List<byte> ret = new();
            Type t = obj.GetType();
            string name = t.ToString();
            var props = t.GetProperties();
            ret.Add((byte)ByteToken.String_UTF8);
            ret.AddRange(WriteString_UTF8(name));
            foreach (var p in props)
            {
                if (p.PropertyType.IsPublic && p.PropertyType.IsPrimitive)
                    ret.AddRange(WriteProperty(p, obj));
                else
                    throw new Exception($"Unsupported type as a member of a custom type: {p.PropertyType.FullName}");
            }
            return ret.ToArray();
        }
        byte[] Write(object val)
        {
            List<byte> r = new();
            if (Token.IsPrimitive(val.GetType()))
            {
                if (Token.IsMonotype(val.GetType()))
                {
                    r.Add((byte)Token.GetByteToke(val.GetType()));
                    r.AddRange(WriteLiteral(val));
                }
                else
                {
                    r.Add((byte)Token.GetByteToke(val.GetType()));
                    r.AddRange(WriteLiteralCollection(val));
                }
            }
            else
            {
                r.Add((byte)ByteToken.CustomStructure);
                r.AddRange(WriteCustomType(val));
            }
            return r.ToArray();
        }
        byte[] WriteLiteral(object val)
        {
            ByteToken dt = Token.GetByteToke(val.GetType());
            List<byte> ret = new();
            ret.AddRange(
                dt switch
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
                    ByteToken.Vector3 => WriteVector3((Vector3)val),
                    ByteToken.Quaternion => WriteQuaternion((Quaternion)val),
                    ByteToken.DateTime => WriteDateTime((DateTime)val),
                    ByteToken.GUID => WriteGuid((Guid)val),
                    ByteToken.CustomStructure => WriteCustomType(val),
                    _ => throw new Exception($"Type not supported : {dt}")
                }
            );
            return ret.ToArray();
        }
        byte[] WriteLiteralCollection(object arr)
        {
            List<byte> r = new();
            var bt = Token.GetByteToke(arr.GetType());
            r.AddRange(
                bt switch
                {
                    ByteToken.Bytes => WriteCollection((ICollection<byte>)arr),
                    ByteToken.SBytes => WriteCollection((ICollection<sbyte>)arr),
                    ByteToken.Shorts => WriteCollection((ICollection<short>)arr),
                    ByteToken.UShorts => WriteCollection((ICollection<ushort>)arr),
                    ByteToken.Ints => WriteCollection((ICollection<int>)arr),
                    ByteToken.UInts => WriteCollection((ICollection<uint>)arr),
                    ByteToken.Longs => WriteCollection((ICollection<long>)arr),
                    ByteToken.ULongs => WriteCollection((ICollection<ulong>)arr),
                    ByteToken.Floats => WriteCollection((ICollection<float>)arr),
                    ByteToken.Doubles => WriteCollection((ICollection<double>)arr),
                    ByteToken.Strings => WriteCollection((ICollection<string>)arr),
                    _ => throw new Exception($"Type {bt} not supported as a collection.")
                }
            );
            return r.ToArray();
        }
        private byte[] WriteCollection<T>(ICollection<T> val)
        {
            List<byte> r = new();
            r.AddRange(LengthWriter(val.Count));
            foreach (var v in val)
            {
                r.AddRange(WriteLiteral(v));
            }
            return r.ToArray();
        }
        byte[] WriteVector3(Vector3 val)
        {
            List<byte> r = new();
            r.AddRange(WriteLiteral(val.x));
            r.AddRange(WriteLiteral(val.y));
            r.AddRange(WriteLiteral(val.z));
            return r.ToArray();
        }
        byte[] WriteQuaternion(Quaternion val)
        {
            List<byte> r = new();
            r.AddRange(WriteLiteral(val.x));
            r.AddRange(WriteLiteral(val.y));
            r.AddRange(WriteLiteral(val.z));
            r.AddRange(WriteLiteral(val.w));
            return r.ToArray();
        }
        byte[] WriteDateTime(DateTime val)
        {
            List<byte> r = new();
            r.AddRange(WriteLiteral(val.Ticks));
            return r.ToArray();
        }
        byte[] WriteGuid(Guid val)
        {
            List<byte> r = new();
            r.AddRange(WriteCollection(val.ToByteArray()));
            return r.ToArray();
        }
    }


}
