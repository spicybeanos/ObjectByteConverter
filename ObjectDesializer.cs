using System;
using System.Text;
using System.Text.Json;
using System.Reflection;

namespace ObjectByteConverter
{
    public class ObjectDesializer
    {
        private List<byte> buffer { get; set; }
        private int pointer { get; set; }
        public ObjectDesializer(ICollection<byte> bytes)
        {
            pointer = 0;
            buffer = new List<byte>();
            buffer.AddRange(bytes);
        }
        /*
            Doesnt work. gives this:
            System.Reflection.TargetException: Non-static method requires a target at 
            property.SetValue(r, vars[property.Name]);

            tf does that mean
            how do i fix this.
            i cant do new T()
            then what am i supposed to do
            please help me

            for now just use Decode() and assign values manually
        */
        private T Deserialize<T>()
        {
#nullable enable
            T? r = default(T);
            Dictionary<string, object> vars = Decode();
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if (vars.ContainsKey(property.Name))
                {
                    property.SetValue(r, vars[property.Name]);
                }
            }
            return r;
        }
        public Dictionary<string, object> Decode()
        {
            Dictionary<string, object> vars = new Dictionary<string, object>();

            while (pointer < buffer.Count)
            {
                ByteToken t = ReadByteToken();
                string identifier = "";
                object value;
                switch (t)
                {
                    case ByteToken.IdentifierName:
                        identifier = ReadString();
                        t = ReadByteToken();
                        break;
                    case ByteToken.SHA1Check:
                        string hash = ReadString();
                        t = ReadByteToken();
                        if (!hashIsSame(hash))
                        {
                            throw new Exception($"Corrupted datagram expected {JsonSerializer.Serialize(hash)}");
                        }
                        break;
                    default:
                        throw new Exception($"Unexpected Token : {t}");
                }
                switch (t)
                {
                    case ByteToken.Int:
                        value = ReadInt();
                        break;
                    case ByteToken.Short:
                        value = ReadShort();
                        break;
                    case ByteToken.Long:
                        value = ReadLong();
                        break;
                    case ByteToken.Byte:
                        value = ReadByte();
                        break;
                    case ByteToken.Char:
                        value = ReadChar();
                        break;
                    case ByteToken.String:
                        value = ReadString();
                        break;
                    case ByteToken.Bool:
                        value = ReadBool();
                        break;
                    case ByteToken.Ints:
                        value = ReadInts();
                        break;
                    case ByteToken.Shorts:
                        value = ReadShorts();
                        break;
                    case ByteToken.Longs:
                        value = ReadLongs();
                        break;
                    case ByteToken.Bytes:
                        value = ReadBytes();
                        break;
                    case ByteToken.Strings:
                        value = ReadStrings();
                        break;
                    default:
                        throw new Exception($"Unexpected type {t}");
                }
                vars.Add(identifier, value);
            }

            return vars;
        }
        bool hashIsSame(string hash)
        {
            List<byte> check = new List<byte>();
            for (int i = pointer; i < buffer.Count; i++)
            {
                check.Add(buffer[i]);
            }
            string h2 = Sha1.Hash(check.ToArray());
            Console.WriteLine($"Got hash:{h2}");
            return h2 == hash;
        }
        int ReadInt()
        {
            int r = BitConverter.ToInt32(buffer.ToArray(), pointer);
            pointer += sizeof(int);
            return r;
        }
        long ReadLong()
        {
            long r = BitConverter.ToInt64(buffer.ToArray(), pointer);
            pointer += sizeof(long);
            return r;
        }
        short ReadShort()
        {
            short r = BitConverter.ToInt16(buffer.ToArray(), pointer);
            pointer += sizeof(short);
            return r;
        }
        byte ReadByte()
        {
            byte r = buffer[pointer];
            pointer += sizeof(byte);
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
            char ch = '\0';
            do
            {
                ch = ReadChar();
                if (ch != '\0')
                    s += ch;
            }
            while (ch != '\0');
            return s;
        }

        ICollection<int> ReadInts()
        {
            int length = ReadInt();
            List<int> r = new List<int>();
            for (int i = 0; i < length; i++)
            {
                r.Add(ReadInt());
            }
            return r;
        }
        ICollection<long> ReadLongs()
        {
            int length = ReadInt();
            List<long> r = new List<long>();
            for (int i = 0; i < length; i++)
            {
                r.Add(ReadLong());
            }
            return r;
        }
        ICollection<short> ReadShorts()
        {
            int length = ReadInt();
            List<short> r = new List<short>();
            for (int i = 0; i < length; i++)
            {
                r.Add(ReadShort());
            }
            return r;
        }
        ICollection<byte> ReadBytes()
        {
            int length = ReadInt();
            List<byte> r = new List<byte>();
            for (int i = 0; i < length; i++)
            {
                r.Add(ReadByte());
            }
            return r;
        }
        ICollection<string> ReadStrings()
        {
            int length = ReadInt();
            List<string> r = new List<string>();
            for (int i = 0; i < length; i++)
            {
                r.Add(ReadString());
            }
            return r;
        }
    }
}