using System.ComponentModel;
using System;
using System.Text;
using System.Text.Json;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace ByteConverter
{
    public class Desializer
    {
        private List<byte> buffer { get; set; }
        public delegate int NumReader(byte[] buff, ref int pointer);
        public NumReader LengthReader { get; set; }
        public Desializer(ICollection<byte> bytes)
        {
            buffer = new();
            buffer.AddRange(bytes);
        }
        object ReadArray(ByteToken type, byte[] buff, ref int pointer)
        {
            int length = LengthReader(buff,ref pointer);
            object[] objs = new object[length];
            for (int i = 0; i < length; i++)
            {
                objs[i] = ReadFixed(type,buff,ref pointer);
            }
            return objs;
        }
        object Meta_ReadArray(ByteToken type, byte[] buff, ref int pointer)
        {
            int length = buff[pointer++];
            object[] objs = new object[length];
            for (int i = 0; i < length; i++)
            {
                objs[i] = ReadFixed(type,buff,ref pointer);
            }
            return objs;
        }
        object ReadFixed(ByteToken type, byte[] buff, ref int pointer)
        {
            object val;
            switch (type)
            {
                case ByteToken.Bool:
                    val = BitConverter.ToBoolean(buff, pointer);
                    pointer += sizeof(byte);
                    break;
                case ByteToken.Byte:
                    val = buff[pointer];
                    pointer++;
                    break;
                case ByteToken.SByte:
                    val = (sbyte)buff[pointer];
                    pointer++;
                    break;
                case ByteToken.Short:
                    val = BitConverter.ToInt16(buff, pointer);
                    pointer += sizeof(short);
                    break;
                case ByteToken.UShort:
                    val = BitConverter.ToUInt16(buff, pointer);
                    pointer += sizeof(ushort);
                    break;
                case ByteToken.Int:
                    val = BitConverter.ToInt32(buff, pointer);
                    pointer += sizeof(int);
                    break;
                case ByteToken.UInt:
                    val = BitConverter.ToUInt32(buff, pointer);
                    pointer += sizeof(uint);
                    break;
                case ByteToken.Long:
                    val = BitConverter.ToInt64(buff, pointer);
                    pointer += sizeof(long);
                    break;
                case ByteToken.ULong:
                    val = BitConverter.ToUInt64(buff, pointer);
                    pointer += sizeof(ulong);
                    break;
                case ByteToken.Char:
                    val = BitConverter.ToChar(buff, pointer);
                    pointer += sizeof(char);
                    break;
                case ByteToken.Float:
                    val = BitConverter.ToSingle(buff, pointer);
                    pointer += sizeof(float);
                    break;
                case ByteToken.Double:
                    val = BitConverter.ToDouble(buff, pointer);
                    pointer += sizeof(double);
                    break;
                case ByteToken.DateTime:
                    val = DateTime.FromBinary(BitConverter.ToInt64(buff, pointer));
                    pointer += sizeof(long);
                    break;
                default:
                    throw new Exception($"Unsupported type :{type}");
            }
            return val;
        }
        string ReadStringUTF8(byte[] buff, ref int pointer)
        {
            int length = LengthReader(buff, ref pointer);
            string val = Encoding.UTF8.GetString(buff, pointer, length);
            pointer += length;
            return val;
        }
        string ReadStringUnicode(byte[] buff, ref int pointer)
        {
            int length = LengthReader(buff, ref pointer);
            string val = Encoding.Unicode.GetString(buff, pointer, length * 2);
            pointer += length * 2;
            return val;
        }
        string Meta_ReadString(byte[] buff, ref int pointer)
        {
            int length = buff[pointer];
            pointer++;
            string val = Encoding.UTF8.GetString(buff, pointer, length);
            pointer += length;
            return val;
        }
        byte[] Meta_ReadBytes(byte[] buff, ref int pointer)
        {
            int length = buff[pointer];
            pointer++;
            byte[] val = new byte[length];
            for (int i = 0; i < length; i++)
            {
                val[i] = (byte)ReadFixed(ByteToken.Byte, buff, ref pointer);
            }
            return val;
        }
        Dictionary<string, object> ReadMetaInf(byte[] buff, ref int pointer)
        {
            Dictionary<string, object> ret = new();
            while (ret.Count < 6)
            {
                var bt = (byte)ReadFixed(ByteToken.Byte, buff, ref pointer);
                if (bt != (byte)ByteToken.IdentifierName)
                    throw new Exception("Excepted an Identifier token! Datagram may be corrupted.");
                string name = Meta_ReadString(buff, ref pointer);
                var type = (ByteToken)(byte)ReadFixed(ByteToken.Byte, buff, ref pointer);
                
            }
            return ret;
        }
    }
}

