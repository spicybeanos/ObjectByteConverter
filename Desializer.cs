using System.ComponentModel;
using System;
using System.Text;
using System.Text.Json;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace ByteConverter
{
    public class Desializer
    {
        private static int ReadInt(byte[] buff, ref int pointer)
        {
            int val = BitConverter.ToInt32(buff, pointer);
            pointer += sizeof(int);
            return val;
        }
        private static string ReadUTF8(byte[] buff, ref int pointer)
        {
            int len_ = ReadInt(buff, ref pointer);
            string val = Encoding.UTF8.GetString(buff, pointer, len_);
            pointer += len_;
            return val;
        }
        public static string GetClassName(byte[] buff, ref int pointer)
        {
            int packlen = ReadInt(buff, ref pointer);
            string className = ReadUTF8(buff, ref pointer);
            return className;
        }
        /// <summary>
        /// Deserializes the buffer. <br/>
        /// Make sure that pointer is at the start of the packet!<br/>
        /// Make sure you put in the correct class to deserialize!<br/>
        /// If you do not know the class which has been serialized, <br/>
        /// use "GetClassName(byte[],ref int)" to know which class has been serialized
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buff">bytes to deserialize</param>
        /// <param name="pointer">points at the start of the packet</param>
        /// <returns></returns>
        public static Result<T> Deserialize<T>(byte[] buff, ref int pointer)
        {
            string cname = "";
            try
            {
                int pklen = ReadInt(buff, ref pointer);
                cname = ReadUTF8(buff, ref pointer);
                string json = ReadUTF8(buff, ref pointer);
                T t = JsonSerializer.Deserialize<T>(json);
                if (t != null)
                {
                    return new Result<T>(true, t);
                }
                else
                {
                    Result<T> r = new(false);
                    r.exception = new Exception($"Could not deserialize {cname} as {nameof(T)}.");
                    return r;
                }
            }
            catch (Exception ex)
            {
                Result<T> r = new(false);
                r.exception = new Exception($"Could not deserialize {cname} as {nameof(T)}:\n{ex}");
                return r;
            }
        }
        public static List<string> DecodePacket(byte[] buff)
        {
            List<string> ret = new();
            int pointer = 0;
            while (pointer < buff.Length)
            {
                int index0 = pointer;
                int pcktLen = ReadInt(buff, ref pointer);
                string className = ReadUTF8(buff,ref pointer);
                string json = ReadUTF8(buff,ref pointer);
                ret.Add(className);
                ret.Add(json);
            }
            return ret;
        }
    }
}

