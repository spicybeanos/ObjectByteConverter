using System.ComponentModel;
using System;
using System.Text;
using System.Text.Json;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace ObjectByteConverter
{
    public class Desializer
    {
        private List<byte> buffer { get; set; }
        private int pointer { get; set; }
        public int DatagramLength { get; private set; }
        public delegate int LengthReader();
        public LengthReader ReadLength;
        public Desializer(ICollection<byte> bytes)
        {
            pointer = 0;
            buffer = new List<byte>();
            buffer.AddRange(bytes);
            ReadLength = () =>
            {
                return ReadUShort();
            };
        }
        public Result<Exception> Deserialize<T>(ref T obj, int start = 0)
        {
            try
            {
                var res = Decode(start);
                if (!res.Success)
                {
                    return new Result<Exception>(false, new Exception($"Could not decode bianary: {res.exception}"));
                }
                var vars = res.Value;
                PropertyInfo[] properties = obj.GetType().GetProperties();
                foreach (var property in properties)
                {
                    if (vars.ContainsKey(property.Name))
                    {
                        property.SetValue(obj, vars[property.Name]);
                    }
                }
                return new Result<Exception>(true, null);
            }
            catch (Exception ex)
            {
                return new Result<Exception>(false, ex);
            }
        }
        public Result<string> Mogus()
        {
            try
            {
                var r__ = DeTape();
                if (r__.Success)
                {
                    var rs = r__.Value;
                    string s_ = "";
                    foreach (object o in rs)
                    {
                        s_ += $"[{o}]";
                    }
                    return new Result<string>(true, s_);
                }
                else
                {
                    return new Result<string>(false, $"Could not get the objects ecoded : {r__.Value[0]}");
                }
            }
            catch (Exception ex)
            {
                return new Result<string>(false, ex.ToString());
            }

        }
        private Result<List<object>> DeTape()
        {
            try
            {
                pointer = 0;
                List<object> ret = new();
                while (pointer < buffer.Count)
                {
                    ByteToken t = ReadByteToken();
                    string identifier = "";
                    object value;
                    ret.Add(t);
                    switch (t)
                    {
                        case ByteToken.IdentifierName:
                            identifier = ReadString();
                            t = ReadByteToken();
                            break;
                        default:
                            throw new Exception($"Unexpected Token : {t} at pointer {pointer}");
                    }
                    ret.Add(identifier);
                    ret.Add(t);
                    value = ReadValue(t);
                    ret.Add(value);
                }
                return new Result<List<object>>(true, ret);
            }
            catch (Exception ex)
            {
                return new Result(ex);
            }
        }

        public Result<Dictionary<string, object>> Decode(int start = 0)
        {
            try
            {
                Dictionary<string, object> vars = new();
                pointer = start;

                /*
                var _dlen = GetDatagramLength(start);
                if (_dlen.Success)
                {
                    DatagramLength = _dlen.Value;
                }
                else
                {
                    return Result<Dictionary<string, object>>.Failed(_dlen.exception);
                }*/

                while (pointer < DatagramLength)
                {
                    ByteToken t = ReadByteToken();
                    string identifier = "";
                    object value;
                    identifier = t switch
                    {
                        ByteToken.IdentifierName => ReadStringASCII(),
                        _ => throw new Exception($"Unexpected Token : {t}"),
                    };
                    t = ReadByteToken();
                    value = ReadValue(t);
                    vars.Add(identifier, value);
                }
                pointer = 0;
                return new Result<Dictionary<string, object>>(true, vars);
            }
            catch (Exception ex)
            {
                return new Result(ex);
            }

        }
        public Result<string> GetClassName()
        {
            var res = Decode();
            if (res.Success)
            {
                if (res.Value.ContainsKey(Serializer.ClassName))
                {
                    return new Result<string>(true, (string)res.Value[Serializer.ClassName]);
                }
                return new Result<string>(false, "Data does not specify class name.");
            }
            return new Result<string>(false, "Failed to decode.");
        }
        public object ReadValue(ByteToken bt)
        {
            object value = bt switch
            {
                ByteToken.Int => ReadInt(),
                ByteToken.UInt => ReadUInt(),
                ByteToken.Float => ReadFloat(),
                ByteToken.Short => ReadShort(),
                ByteToken.UShort => ReadUShort(),
                ByteToken.Long => ReadLong(),
                ByteToken.ULong => ReadULong(),
                ByteToken.Double => ReadDouble(),
                ByteToken.Vector3 => ReadVector3(),
                ByteToken.Quaternion => ReadQuaternion(),
                ByteToken.DateTime => ReadDateTime(),
                ByteToken.Byte => ReadByte(),
                ByteToken.Char => ReadChar(),
                ByteToken.String => ReadString(),
                ByteToken.String_ASCII => ReadStringASCII(),
                ByteToken.Bool => ReadBool(),
                ByteToken.Ints => ReadInts(),
                ByteToken.Longs => ReadLongs(),
                ByteToken.Shorts => ReadShorts(),
                ByteToken.UInts => ReadUInts(),
                ByteToken.ULongs => ReadULongs(),
                ByteToken.UShorts => ReadUShorts(),
                ByteToken.Floats => ReadFloats(),
                ByteToken.Doubles => ReadDoubles(),
                ByteToken.Bytes => ReadBytes(),
                ByteToken.GUID => ReadGuid(),
                ByteToken.Strings => ReadStrings(),
                _ => throw new Exception($"Unexpected type {bt}"),
            };
            return value;
        }
        public void SetLengthReader(ByteToken bt)
        {
            switch (bt)
            {
                case ByteToken.Short:
                    ReadLength = (() => { return (int)ReadShort(); });
                    break;
                case ByteToken.UShort:
                    ReadLength = (() => { return (int)ReadUShort(); });
                    break;
                case ByteToken.Int:
                    ReadLength = (() => { return (int)ReadInt(); });
                    break;
                case ByteToken.UInt:
                    ReadLength = (() => { return (int)ReadUInt(); });
                    break;
                case ByteToken.Long:
                    ReadLength = (() => { return (int)ReadLong(); });
                    break;
                case ByteToken.ULong:
                    ReadLength = (() => { return (int)ReadULong(); });
                    break;
                default:
                    ReadLength = (() => { return (int)ReadUShort(); });
                    break;
            }
        }
        public bool IsShaVerified()
        {
            var res = Decode();
            if (res.Success)
            {
                if (res.Value.ContainsKey(Serializer.ShaVerified))
                {
                    return (bool)res.Value[Serializer.ShaVerified];
                }
            }
            return false;
        }
        public Result<string> GetProvidedHash()
        {
            var res = Decode();
            if (res.Success)
            {
                if (res.Value.ContainsKey(Serializer.ShaVerified))
                {
                    if ((bool)res.Value[Serializer.ShaVerified])
                    {
                        if (res.Value.ContainsKey(Serializer.ShaVerificationCode))
                        {
                            return new Result<string>(true, (string)res.Value[Serializer.ShaVerificationCode]);
                        }
                        return new Result<string>(false, $"{Serializer.ShaVerificationCode} not found.");
                    }
                    return new Result<string>(false, $"Data is not hashed verified.{Serializer.ShaVerified} is false.");
                }
                return new Result<string>(false, $"{Serializer.ShaVerified} not found.");
            }
            return new Result<string>(false, "Decoding failed.");
        }
        public Result<string> GenerateCheckHash()
        {
            if (IsShaVerified())
            {
                ReadByteToken();
                ReadString();
                ReadByteToken();
                ReadBool();
                ReadByteToken();
                ReadString();
                ReadByteToken();
                ReadString();
            }
            else
            {
                ReadByteToken();
                ReadString();
                ReadByteToken();
                ReadBool();
            }
            List<byte> byts = new List<byte>();
            for (int i = pointer; i < buffer.Count; i++)
            {
                byts.Add(buffer[i]);
            }
            byte[] hash = GenSha.Sha1Hash(byts.ToArray());
            return new Result<string>(true, GenSha.ByteArrayToString(hash));
        }
        /*
        public Result<int> GetDatagramLength(int start = 0)
        {
            try
            {
                for (pointer = start; pointer < buffer.Count;)
                {
                    ByteToken t = ReadByteToken();
                    string identifier = "";
                    object value;
                    switch (t)
                    {
                        case ByteToken.IdentifierName:
                            identifier = ReadString();
                            break;
                        case ByteToken.DatagramLength:
                            identifier = ObjectSerializer.DatagramLength;
                            t = ReadByteToken();
                            return new Result<int>(true, (ushort)ReadValue(t));
                        default:
                            return Result<int>.Failed(new Exception($"Unexpected Token : {t}"));
                    }
                    t = ReadByteToken();
                    value = ReadValue(t);
                }
                return Result<int>.Failed(new Exception($"Could not find DatagramLength token which is {ByteToken.DatagramLength}"));
            }
            catch (Exception ex)
            {
                return Result<int>.Failed(ex);
            }

        }*/
        public bool IsDataIntact()
        {
            if (IsShaVerified())
            {
                var s1 = GetProvidedHash();
                var s2 = GenerateCheckHash();
                if (s1.Success && s2.Success)
                    if (s1.Value == s2.Value)
                        return true;
            }
            return false;
        }

        public List<byte> SubArray(int from, int end)
        {
            if (end < 0)
            {
                return null;
            }
            List<byte> r = new List<byte>();
            for (int i = from; i < buffer.Count && i < end; i++)
            {
                r.Add(buffer[i]);
            }
            return r;
        }

        public Result<List<Desializer>> Partition()
        {
            try
            {
                pointer = 0;
                int start = 0;
                List<Desializer> des = new List<Desializer>();
                while (pointer < buffer.Count)
                {
                    ByteToken t = ReadByteToken();
                    if (t == ByteToken.EOF)
                    {
                        List<byte> r = SubArray(start,pointer);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Result(ex);
            }
        }


        int ReadInt()
        {
            int r = BitConverter.ToInt32(buffer.ToArray(), pointer);
            pointer += sizeof(int);
            return r;
        }
        uint ReadUInt()
        {
            uint r = BitConverter.ToUInt32(buffer.ToArray(), pointer);
            pointer += sizeof(uint);
            return r;
        }
        float ReadFloat()
        {
            float t = BitConverter.ToSingle(buffer.ToArray(), pointer);
            pointer += sizeof(float);
            return t;
        }
        long ReadLong()
        {
            long r = BitConverter.ToInt64(buffer.ToArray(), pointer);
            pointer += sizeof(long);
            return r;
        }
        ulong ReadULong()
        {
            ulong r = BitConverter.ToUInt64(buffer.ToArray(), pointer);
            pointer += sizeof(ulong);
            return r;
        }
        double ReadDouble()
        {
            double r = BitConverter.ToDouble(buffer.ToArray(), pointer);
            pointer += sizeof(double);
            return r;
        }
        short ReadShort()
        {
            short r = BitConverter.ToInt16(buffer.ToArray(), pointer);
            pointer += sizeof(short);
            return r;
        }
        ushort ReadUShort()
        {
            ushort r = BitConverter.ToUInt16(buffer.ToArray(), pointer);
            pointer += sizeof(ushort);
            return r;
        }
        byte ReadByte()
        {
            byte r = buffer[pointer];
            pointer += sizeof(byte);
            return r;
        }
        Vector3 ReadVector3()
        {
            Vector3 r = new Vector3();
            r.x = ReadFloat();
            r.y = ReadFloat();
            r.z = ReadFloat();
            return r;
        }
        Quaternion ReadQuaternion()
        {
            Quaternion r = new Quaternion();
            r.x = ReadFloat();
            r.y = ReadFloat();
            r.z = ReadFloat();
            r.w = ReadFloat();
            return r;
        }
        bool ReadBool()
        {
            bool b = BitConverter.ToBoolean(buffer.ToArray(), pointer);
            pointer += sizeof(bool);
            return b;
        }
        char ReadChar()
        {
            char r = BitConverter.ToChar(buffer.ToArray(), pointer);
            pointer += sizeof(char);
            return r;
        }
        ByteToken ReadByteToken()
        {
            ByteToken r = (ByteToken)ReadByte();
            return r;
        }
        string ReadString()
        {
            string s = "";
            int len = ReadLength();
            for (int i = 0; i < len; i++)
            {
                char ch = ReadChar();
                s += ch;
            }
            return s;
        }
        DateTime ReadDateTime()
        {
            long d = ReadLong();
            return new DateTime(d);
        }
        Guid ReadGuid()
        {
            return new Guid(ReadBytes().ToArray());
        }
        string ReadStringASCII()
        {
            int len = ReadLength();
            return Encoding.ASCII.GetString(buffer.ToArray(), pointer, len);
        }


        ICollection<int> ReadInts()
        {
            int length = ReadLength();
            List<int> r = new List<int>();
            for (int i = 0; i < length; i++)
            {
                r.Add(ReadInt());
            }
            return r;
        }
        ICollection<uint> ReadUInts()
        {
            int length = ReadLength();
            List<uint> r = new List<uint>();
            for (int i = 0; i < length; i++)
            {
                r.Add(ReadUInt());
            }
            return r;
        }
        ICollection<float> ReadFloats()
        {
            int length = ReadLength();
            List<float> r = new List<float>();
            for (int i = 0; i < length; i++)
            {
                r.Add(ReadFloat());
            }
            return r;
        }
        ICollection<double> ReadDoubles()
        {
            int length = ReadLength();
            List<double> r = new List<double>();
            for (int i = 0; i < length; i++)
            {
                r.Add(ReadDouble());
            }
            return r;
        }
        ICollection<long> ReadLongs()
        {
            int length = ReadLength();
            List<long> r = new List<long>();
            for (int i = 0; i < length; i++)
            {
                r.Add(ReadLong());
            }
            return r;
        }
        ICollection<short> ReadShorts()
        {
            int length = ReadLength();
            List<short> r = new List<short>();
            for (int i = 0; i < length; i++)
            {
                r.Add(ReadShort());
            }
            return r;
        }
        ICollection<ulong> ReadULongs()
        {
            int length = ReadLength();
            List<ulong> r = new List<ulong>();
            for (int i = 0; i < length; i++)
            {
                r.Add(ReadULong());
            }
            return r;
        }
        ICollection<ushort> ReadUShorts()
        {
            int length = ReadLength();
            List<ushort> r = new List<ushort>();
            for (int i = 0; i < length; i++)
            {
                r.Add(ReadUShort());
            }
            return r;
        }
        ICollection<byte> ReadBytes()
        {
            int length = ReadLength();
            List<byte> r = new List<byte>();
            for (int i = 0; i < length; i++)
            {
                r.Add(ReadByte());
            }
            return r;
        }
        ICollection<string> ReadStrings()
        {
            int length = ReadLength();
            List<string> r = new List<string>();
            for (int i = 0; i < length; i++)
            {
                r.Add(ReadString());
            }
            return r;
        }
    }
}

