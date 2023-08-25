using System;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace ByteConverter
{
    public enum ByteToken
    {
        EOF=255,
        IdentifierName=1,
        SHA1Check,
        Int,
        UInt,
        Long,
        ULong,
        Short,
        UShort,
        Byte,
        SByte,
        Float,
        Double,
        Bool,
        Char,
        DateTime,
        String,
        String_ASCII,
        Ints,
        UInts,
        Longs,
        ULongs,
        Shorts,
        UShorts,
        Bytes,
        Floats,
        Doubles,
        GUID,
        Strings,
        Vector3,
        Quaternion
    }

    public class Token
    {
        public ByteToken type { get; set; }
        public object value { get; set; }

        public static readonly Dictionary<Type, ByteToken> DataType = new Dictionary<Type, ByteToken>(){
            {typeof(int),ByteToken.Int},
            {typeof(uint),ByteToken.UInt},
            {typeof(byte),ByteToken.Byte},
            {typeof(sbyte),ByteToken.SByte},
            {typeof(short),ByteToken.Short},
            {typeof(ushort),ByteToken.UShort},
            {typeof(float),ByteToken.Float},
            {typeof(double),ByteToken.Double},
            {typeof(bool),ByteToken.Bool},
            {typeof(long),ByteToken.Long},
            {typeof(ulong),ByteToken.ULong},
            {typeof(DateTime),ByteToken.DateTime},
            {typeof(char),ByteToken.Char},
            {typeof(string),ByteToken.String},
            {typeof(Vector3),ByteToken.Vector3},
            {typeof(Quaternion),ByteToken.Quaternion},

            {typeof(ICollection<int>),ByteToken.Ints},
            {typeof(ICollection<long>),ByteToken.Longs},
            {typeof(ICollection<short>),ByteToken.Shorts},
            {typeof(ICollection<uint>),ByteToken.UInts},
            {typeof(ICollection<ulong>),ByteToken.ULongs},
            {typeof(ICollection<ushort>),ByteToken.UShorts},
            {typeof(ICollection<byte>),ByteToken.Bytes},
            {typeof(ICollection<float>),ByteToken.Floats},
            {typeof(ICollection<double>),ByteToken.Doubles},
            {typeof(ICollection<string>),ByteToken.Strings},

            {typeof(int[]),ByteToken.Ints},
            {typeof(long[]),ByteToken.Longs},
            {typeof(short[]),ByteToken.Shorts},
            {typeof(uint[]),ByteToken.UInts},
            {typeof(ulong[]),ByteToken.ULongs},
            {typeof(ushort[]),ByteToken.UShorts},
            {typeof(byte[]),ByteToken.Bytes},
            {typeof(Guid),ByteToken.GUID},
            {typeof(float[]),ByteToken.Floats},
            {typeof(double[]),ByteToken.Doubles},
            {typeof(string[]),ByteToken.Strings},

            {typeof(List<int>),ByteToken.Ints},
            {typeof(List<long>),ByteToken.Longs},
            {typeof(List<short>),ByteToken.Shorts},
            {typeof(List<uint>),ByteToken.UInts},
            {typeof(List<ulong>),ByteToken.ULongs},
            {typeof(List<ushort>),ByteToken.UShorts},
            {typeof(List<byte>),ByteToken.Bytes},
            {typeof(List<float>),ByteToken.Floats},
            {typeof(List<double>),ByteToken.Doubles},
            {typeof(List<string>),ByteToken.Strings},
        };
    }
}