using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ObjectByteConverter
{
    public class ObjectSerializer
    {
        public List<byte> buffer { get; set; }
        public ObjectSerializer()
        {
            buffer = new List<byte>();
        }
        public List<byte> SerializeProperties(object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            string className = obj.GetType().ToString();
            WriteLiteral((byte)ByteToken.IdentifierName);
            WriteLiteral("$ClassName");
            WriteLiteral((byte)ByteToken.String);
            WriteLiteral(className);
            foreach (var property in properties)
            {
                if (Token.DataType.ContainsKey(property.PropertyType))
                {
                    WriteLiteral((byte)ByteToken.IdentifierName);
                    WriteLiteral(property.Name);
                    WriteLiteral((byte)Token.DataType[property.PropertyType]);
                    Write(property.GetValue(obj));
                }
                else{
                    //impliment compund deserialization
                    throw new Exception($"Unsupported type : {property.PropertyType.ToString()}");                    
                }
            }
            return buffer;
        }
        private string SerializeProperties_trial(object obj)
        {
            string str = "";
            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (var property in properties)
            {
                str += $"[{(ByteToken.IdentifierName)}]";
                str += $"[{(property.Name)}]";
                str += $"[{(Token.DataType[property.PropertyType])}]";
                str += $"[{(property.GetValue(obj))}]\n";
            }
            return str;
        }
        void Write(Object obj)
        {
            if (obj is int)
            {
                WriteLiteral((int)obj);
            }
            else if (obj is short)
            {
                WriteLiteral((short)obj);
            }
            else if (obj is byte)
            {
                WriteLiteral((byte)obj);
            }
            else if (obj is long)
            {
                WriteLiteral((long)obj);
            }
            else if (obj is char)
            {
                WriteLiteral((char)obj);
            }
            else if (obj is string)
            {
                WriteLiteral((string)obj);
            }
            else if (obj is bool)
            {
                WriteLiteral((bool)obj);
            }
            else if (obj is ICollection<int>)
            {
                WriteLiteral((ICollection<int>)obj);
            }
            else if (obj is ICollection<short>)
            {
                WriteLiteral((ICollection<short>)obj);
            }
            else if (obj is ICollection<byte>)
            {
                WriteLiteral((ICollection<byte>)obj);
            }
            else if (obj is ICollection<long>)
            {
                WriteLiteral((ICollection<long>)obj);
            }
            else if (obj is ICollection<string>)
            {
                WriteLiteral((ICollection<string>)obj);
            }
            else
            {
                throw new Exception($"Unsupported type : {obj.GetType()}");
            }
        }
        void WriteLiteral(int val)
        {
            buffer.AddRange(BitConverter.GetBytes(val));
        }
        void WriteLiteral(byte val)
        {
            buffer.Add(val);
        }
        void WriteLiteral(long val)
        {
            buffer.AddRange(BitConverter.GetBytes(val));
        }
        void WriteLiteral(short val)
        {
            buffer.AddRange(BitConverter.GetBytes(val));
        }
        void WriteLiteral(char val)
        {
            buffer.AddRange(BitConverter.GetBytes(val));
        }
        void WriteLiteral(bool val)
        {
            buffer.AddRange(BitConverter.GetBytes(val));
        }
        void WriteLiteral(string val)
        {
            buffer.AddRange(Encoding.Unicode.GetBytes(val));
            WriteLiteral('\0');
        }
        void WriteLiteral(ICollection<int> val)
        {
            WriteLiteral(val.Count());
            foreach (var item in val)
            {
                WriteLiteral(item);
            }
        }
        void WriteLiteral(ICollection<long> val)
        {
            WriteLiteral(val.Count());
            foreach (var item in val)
            {
                WriteLiteral(item);
            }
        }
        void WriteLiteral(ICollection<short> val)
        {
            WriteLiteral(val.Count());
            foreach (var item in val)
            {
                WriteLiteral(item);
            }
        }
        void WriteLiteral(ICollection<byte> val)
        {
            WriteLiteral(val.Count());
            foreach (var item in val)
            {
                WriteLiteral(item);
            }
        }
        void WriteLiteral(ICollection<string> val)
        {
            WriteLiteral(val.Count());
            foreach (var item in val)
            {
                WriteLiteral(item);
            }
        }
        void WriteLiteral<T,K>(IDictionary<T,K> val){
            
        }
    }
}
