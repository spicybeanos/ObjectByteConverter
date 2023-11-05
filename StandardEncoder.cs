using System.Collections;
using System.Text;

namespace ByteConverter
{
    public enum StringEncodingMode
    {
        UTF8,
        Unicode,
        ASCII
    }
    public class StandardEncoder
    {
        public DataTypeIDs SizeT { get; private set; }
        public StringEncodingMode stringEncoding { get; private set; }
        public StandardEncoder(DataTypeIDs size_t, StringEncodingMode mode)
        {
            SizeT = size_t;
            stringEncoding = mode;
        }
        /// <summary>
        /// Encodes length according to the size in SizeT
        /// to bytes
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] EncodeLength(int length)
        {
            switch (SizeT)
            {
                case DataTypeIDs.Int8:
                    return new byte[] { (byte)length };
                case DataTypeIDs.SInt8:
                    return new byte[] { (byte)length };
                case DataTypeIDs.Int16:
                    return BitConverter.GetBytes((short)length);
                case DataTypeIDs.UInt16:
                    return BitConverter.GetBytes((ushort)length);
                case DataTypeIDs.Int32:
                    return BitConverter.GetBytes(length);
                case DataTypeIDs.UInt32:
                    return BitConverter.GetBytes((uint)length);
                case DataTypeIDs.Int64:
                    return BitConverter.GetBytes((long)length);
                case DataTypeIDs.UInt64:
                    return BitConverter.GetBytes((ulong)length);
                default:
                    throw new Exception($"Invalid length encoder type : {SizeT}");
            }
        }
        public byte[] EncodeFixed(object value, DataTypeIDs dataType)
        {
            if ((int)dataType < (int)DataTypeIDs.Char ||
            (int)dataType > (int)DataTypeIDs.Float64)
                throw new Exception($"Data type {dataType} is not a fixed length data type!");

            List<byte> ret = new()
            {
                (byte)dataType
            };
            switch (dataType)
            {
                case DataTypeIDs.Char:
                    ret.AddRange(BitConverter.GetBytes((char)value));
                    break;
                case DataTypeIDs.Boolean:
                    ret.AddRange(BitConverter.GetBytes((bool)value));
                    break;
                case DataTypeIDs.Int8:
                    ret.AddRange(BitConverter.GetBytes((byte)value));
                    break;
                case DataTypeIDs.SInt8:
                    ret.AddRange(BitConverter.GetBytes((sbyte)value));
                    break;
                case DataTypeIDs.Int16:
                    ret.AddRange(BitConverter.GetBytes((short)value));
                    break;
                case DataTypeIDs.UInt16:
                    ret.AddRange(BitConverter.GetBytes((ushort)value));
                    break;
                case DataTypeIDs.Int32:
                    ret.AddRange(BitConverter.GetBytes((int)value));
                    break;
                case DataTypeIDs.UInt32:
                    ret.AddRange(BitConverter.GetBytes((uint)value));
                    break;
                case DataTypeIDs.Int64:
                    ret.AddRange(BitConverter.GetBytes((long)value));
                    break;
                case DataTypeIDs.UInt64:
                    ret.AddRange(BitConverter.GetBytes((ulong)value));
                    break;
                case DataTypeIDs.Float16:
                    ret.AddRange(BitConverter.GetBytes((Half)value));
                    break;
                case DataTypeIDs.Float32:
                    ret.AddRange(BitConverter.GetBytes((float)value));
                    break;
                case DataTypeIDs.Float64:
                    ret.AddRange(BitConverter.GetBytes((double)value));
                    break;
                default:
                    throw new Exception($"Data type {dataType} is not supported as a fixed data type");
            }
            return ret.ToArray();
        }
        public byte[] EncodeArray(object value, DataTypeIDs dataType)
        {
            if ((int)dataType < (int)DataTypeIDs.Int8_array ||
            (int)dataType > (int)DataTypeIDs.String_array)
                throw new Exception($"Data type {dataType} is not a dynamic data type!");

            List<byte> ret = new();

            switch (dataType)
            {
                case DataTypeIDs.Int8_array:
                    {
                        var arr = (IEnumerable<byte>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeIDs.SInt8_array:
                    {
                        var arr = (IEnumerable<sbyte>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeIDs.Int16_array:
                    {
                        var arr = (IEnumerable<short>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeIDs.UInt16_array:
                    {
                        var arr = (IEnumerable<ushort>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeIDs.Int32_array:
                    {
                        var arr = (IEnumerable<int>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeIDs.UInt32_array:
                    {
                        var arr = (IEnumerable<uint>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeIDs.Int64_array:
                    {
                        var arr = (IEnumerable<long>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeIDs.UInt64_array:
                    {
                        var arr = (IEnumerable<ulong>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeIDs.Float16_array:
                    {
                        var arr = (IEnumerable<Half>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeIDs.Float32_array:
                    {
                        var arr = (IEnumerable<float>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeIDs.Float64_array:
                    {
                        var arr = (IEnumerable<double>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeIDs.String:
                    {
                        var arr = (string)value;
                        int len = arr.Length;
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeString(arr));
                    }
                    break;
                case DataTypeIDs.String_array:
                    {
                        var arr = (string[])value;
                        int len = arr.Length;
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeStringArray(arr));
                    }
                    break;
            }

            return ret.ToArray();
        }
        private byte[] EncodeFixedArray<T>(IEnumerable<T> array)
        {
            List<byte> ret = new();
            foreach (var item in array)
            {
                ret.AddRange(EncodeFixed(item,
                DataTypes.TypeToDataTypeID(typeof(T))));
            }
            return ret.ToArray();
        }
        private byte[] EncodeStringArray(string[] array)
        {
            List<byte> ret = new();
            for (int i = 0; i < array.Length; i++)
            {
                ret.AddRange(EncodeLength(array[i].Length));
                ret.AddRange(EncodeString(array[i]));
            }
            return ret.ToArray();
        }
        private byte[] EncodeString(string str)
        {
            switch (stringEncoding)
            {
                case StringEncodingMode.Unicode:
                    return Encoding.Unicode.GetBytes(str);
                case StringEncodingMode.UTF8:
                    return Encoding.UTF8.GetBytes(str);
                case StringEncodingMode.ASCII:
                    return Encoding.ASCII.GetBytes(str);
                default:
                    throw new Exception($"Invalid string encoding {stringEncoding}!");
            }
        }
    }
}