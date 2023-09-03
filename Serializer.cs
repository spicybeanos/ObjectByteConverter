using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.Json;

namespace ByteConverter
{
    public class Serializer
    {
        private List<byte> buffer { get; set; }
        public Serializer()
        {
            buffer = new();
        }
        public byte[] Serialize(object obj)
        {
            string json = JsonSerializer.Serialize(obj);
            string className = obj.GetType().ToString();
            List<byte> r = new();
            var cn = WriteUtf8(className);
            var js = WriteUtf8(json);
            int len_ = sizeof(int) + cn.Length + js.Length;
            r.AddRange(WriteInt(len_));
            r.AddRange(cn);
            r.AddRange(js);
            return r.ToArray();
        }
        byte[] WriteInt(int val){
            return BitConverter.GetBytes(val);
        }
        byte[] WriteUtf8(string val){
            List<byte> r = new();
            r.AddRange(WriteInt((ushort)val.Length));
            r.AddRange(Encoding.UTF8.GetBytes(val));
            return r.ToArray();
        }
    }
}
