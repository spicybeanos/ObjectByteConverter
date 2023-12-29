using System.Collections;
using System.Text;

namespace ByteConverter
{

    public class PrimitiveEncoder
    {
        public DataTypeID SizeT { get; private set; }
        public StringEncodingMode stringEncoding { get; private set; }
        public MetaInf EncoderMetaInf
        {
            get
            {
                return new()
                {
                    SizeT = SizeT,
                    stringEncodingMode = stringEncoding
                };
            }
        }
        public PrimitiveEncoder(DataTypeID size_t, StringEncodingMode mode)
        {
            SizeT = size_t;
            stringEncoding = mode;
        }
        public PrimitiveEncoder(MetaInf inf)
        {
            SizeT = inf.SizeT;
            stringEncoding = inf.stringEncodingMode;
        }

        public byte[] EncodePrimitive(object value, DataTypeID type)
        {
            if (DataTypes.IsFixed(type))
                return EncodeFixed(value, type);
            else if (DataTypes.IsPrimitiveArray(type))
                return EncodeArray(value, type);
            else
                throw new Exception($"{type} is not a primitive type! Cannot use {nameof(EncodePrimitive)} to encode user defined types");
        }
        public byte[] EncodePrimitive<T>(T value)
        {
            return EncodePrimitive(value, DataTypes.GetDataTypeIDFromType(typeof(T)));
        }
        /// <summary>
        /// Encodes length according to the size in SizeT
        /// to bytes
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private byte[] EncodeLength(long length)
        {
            switch (SizeT)
            {
                case DataTypeID.Int8:
                    return new byte[] { (byte)length };
                case DataTypeID.SInt8:
                    return new byte[] { (byte)length };
                case DataTypeID.Int16:
                    return BitConverter.GetBytes((short)length);
                case DataTypeID.UInt16:
                    return BitConverter.GetBytes((ushort)length);
                case DataTypeID.Int32:
                    return BitConverter.GetBytes((int)length);
                case DataTypeID.UInt32:
                    return BitConverter.GetBytes((uint)length);
                case DataTypeID.Int64:
                    return BitConverter.GetBytes((long)length);
                case DataTypeID.UInt64:
                    return BitConverter.GetBytes((ulong)length);
                default:
                    throw new Exception($"Invalid length encoder type : {SizeT}");
            }
        }
        public byte[] EncodeSizeT(int value)
        {
            return EncodeLength(value);
        }
        private byte[] EncodeFixed(object value, DataTypeID dataType)
        {
            if ((int)dataType < (int)DataTypeID.Char ||
            (int)dataType > (int)DataTypeID.Float64)
                throw new Exception($"Data type {dataType} is not a fixed length data type!");

            List<byte> ret = new();
            switch (dataType)
            {
                case DataTypeID.Char:
                    ret.AddRange(BitConverter.GetBytes((char)value));
                    break;
                case DataTypeID.Boolean:
                    ret.AddRange(BitConverter.GetBytes((bool)value));
                    break;
                case DataTypeID.Int8:
                    ret.AddRange(BitConverter.GetBytes((byte)value));
                    break;
                case DataTypeID.SInt8:
                    ret.AddRange(BitConverter.GetBytes((sbyte)value));
                    break;
                case DataTypeID.Int16:
                    ret.AddRange(BitConverter.GetBytes((short)value));
                    break;
                case DataTypeID.UInt16:
                    ret.AddRange(BitConverter.GetBytes((ushort)value));
                    break;
                case DataTypeID.Int32:
                    ret.AddRange(BitConverter.GetBytes((int)value));
                    break;
                case DataTypeID.UInt32:
                    ret.AddRange(BitConverter.GetBytes((uint)value));
                    break;
                case DataTypeID.Int64:
                    ret.AddRange(BitConverter.GetBytes((long)value));
                    break;
                case DataTypeID.UInt64:
                    ret.AddRange(BitConverter.GetBytes((ulong)value));
                    break;
                case DataTypeID.Float32:
                    ret.AddRange(BitConverter.GetBytes((float)value));
                    break;
                case DataTypeID.Float64:
                    ret.AddRange(BitConverter.GetBytes((double)value));
                    break;
                default:
                    throw new Exception($"Data type {dataType} is not supported as a fixed data type");
            }
            return ret.ToArray();
        }
        private byte[] EncodeArray(object value, DataTypeID dataType)
        {
            if ((int)dataType < (int)DataTypeID.Int8_array ||
            (int)dataType > (int)DataTypeID.String_array)
                throw new Exception($"Data type {dataType} is not a dynamic data type!");

            List<byte> ret = new();

            switch (dataType)
            {
                case DataTypeID.Int8_array:
                    {
                        var arr = (IEnumerable<byte>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeID.SInt8_array:
                    {
                        var arr = (IEnumerable<sbyte>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeID.Int16_array:
                    {
                        var arr = (IEnumerable<short>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeID.UInt16_array:
                    {
                        var arr = (IEnumerable<ushort>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeID.Int32_array:
                    {
                        var arr = (IEnumerable<int>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeID.UInt32_array:
                    {
                        var arr = (IEnumerable<uint>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeID.Int64_array:
                    {
                        var arr = (IEnumerable<long>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeID.UInt64_array:
                    {
                        var arr = (IEnumerable<ulong>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeID.Float32_array:
                    {
                        var arr = (IEnumerable<float>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeID.Float64_array:
                    {
                        var arr = (IEnumerable<double>)value;
                        int len = arr.Count();
                        ret.AddRange(EncodeLength(len));
                        ret.AddRange(EncodeFixedArray(arr));
                    }
                    break;
                case DataTypeID.StdString:
                    {
                        var arr = (string)value;
                        byte[] k = EncodeString(arr);
                        ret.AddRange(EncodeLength(k.LongLength));
                        ret.AddRange(k);
                    }
                    break;
                case DataTypeID.String_ascii:
                    {
                        var arr = (string)value;
                        byte[] k = EncodeStringASCII(arr);
                        ret.AddRange(EncodeLength(k.LongLength));
                        ret.AddRange(k);
                    }
                    break;
                case DataTypeID.String_array:
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
                DataTypes.GetDataTypeIDFromType(typeof(T))));
            }
            return ret.ToArray();
        }
        private byte[] EncodeStringArray(string[] array)
        {
            List<byte> ret = new();
            for (int i = 0; i < array.Length; i++)
            {
                byte[] k = EncodeString(array[i]);
                ret.AddRange(EncodeLength(k.LongLength));
                ret.AddRange(k);
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
        private byte[] EncodeStringASCII(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
    }
}