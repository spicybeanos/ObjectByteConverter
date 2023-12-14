using System.Text;

namespace ByteConverter
{
    public class PrimitiveDecoder
    {
        public StringEncodingMode stringEncoding { get; private set; }
        public DataTypeID sizeT { get; private set; }
        public MetaInf DecoderMetaInf
        {
            get
            {
                return new()
                {
                    SizeTReader = sizeT,
                    stringEncodingMode = stringEncoding
                };
            }
        }
        public PrimitiveDecoder(DataTypeID size_t, StringEncodingMode stringEncodingMode)
        {
            stringEncoding = stringEncodingMode;
            sizeT = size_t;
        }
        public PrimitiveDecoder(MetaInf inf)
        {
            sizeT = inf.SizeTReader;
            stringEncoding = inf.stringEncodingMode;
        }
        public object DecodePrimitive(byte[] data, ref int pointer, DataTypeID type)
        {
            if (type == DataTypeID.Null)
                return null;
            else if (DataTypes.IsFixed(type))
                return ReadFixed(data, ref pointer, type);
            else if (DataTypes.IsPrimitiveArray(type))
                return ReadArray(data, ref pointer, type);
            else
                throw new Exception($"{type} is not a primitive type!");
        }
        public T DecodePrimitive<T>(byte[] data, ref int pointer)
        {
            return (T)DecodePrimitive(data, ref pointer, DataTypes.GetDataTypeIDFromType(typeof(T)));
        }
        public int DecodeSizeT(byte[] data, ref int pointer)
        {
            return ReadLength(data, ref pointer);
        }
        public DataTypeID DecodeDatatypeID(byte[] data, ref int pointer)
        {
            return (DataTypeID)data[pointer++];
        }
        private int ReadLength(byte[] data, ref int pointer)
        {
            long val_ = 0;
            switch (sizeT)
            {
                case DataTypeID.Int8:
                    val_ = data[pointer];
                    pointer += sizeof(byte);
                    break;
                case DataTypeID.SInt8:
                    val_ = data[pointer];
                    pointer += sizeof(sbyte);
                    break;
                case DataTypeID.Int16:
                    val_ = BitConverter.ToInt16(data, pointer);
                    pointer += sizeof(short);
                    break;
                case DataTypeID.UInt16:
                    val_ = BitConverter.ToUInt16(data, pointer);
                    pointer += sizeof(ushort);
                    break;
                case DataTypeID.Int32:
                    val_ = BitConverter.ToInt32(data, pointer);
                    pointer += sizeof(int);
                    break;
                case DataTypeID.UInt32:
                    val_ = BitConverter.ToUInt32(data, pointer);
                    pointer += sizeof(uint);
                    break;
                case DataTypeID.Int64:
                    val_ = BitConverter.ToInt64(data, pointer);
                    pointer += sizeof(long);
                    break;
                case DataTypeID.UInt64:
                    val_ = (long)BitConverter.ToUInt64(data, pointer);
                    pointer += sizeof(ulong);
                    break;
                default:
                    throw new Exception($"Datatype {sizeT} is not a valid length encoding!");
            }
            return (int)val_;
        }
        private T ReadFixed<T>(byte[] data, ref int pointer)
        {
            return (T)ReadFixed(data, ref pointer, DataTypes.GetDataTypeIDFromType(typeof(T)));
        }
        private object ReadFixed(byte[] data, ref int pointer, DataTypeID type)
        {
            if (!DataTypes.IsFixed(type))
                throw new Exception($"Datatype {type} is not a fixed type!");
            object val_;
            switch (type)
            {
                case DataTypeID.Char:
                    val_ = BitConverter.ToChar(data, pointer);
                    pointer += sizeof(char);
                    break;
                case DataTypeID.Boolean:
                    val_ = BitConverter.ToBoolean(data, pointer);
                    pointer += sizeof(bool);
                    break;
                case DataTypeID.Int8:
                    val_ = data[pointer];
                    pointer += sizeof(byte);
                    break;
                case DataTypeID.SInt8:
                    val_ = data[pointer];
                    pointer += sizeof(sbyte);
                    break;
                case DataTypeID.Int16:
                    val_ = BitConverter.ToInt16(data, pointer);
                    pointer += sizeof(short);
                    break;
                case DataTypeID.UInt16:
                    val_ = BitConverter.ToUInt16(data, pointer);
                    pointer += sizeof(ushort);
                    break;
                case DataTypeID.Int32:
                    val_ = BitConverter.ToInt32(data, pointer);
                    pointer += sizeof(int);
                    break;
                case DataTypeID.UInt32:
                    val_ = BitConverter.ToUInt32(data, pointer);
                    pointer += sizeof(uint);
                    break;
                case DataTypeID.Int64:
                    val_ = BitConverter.ToInt64(data, pointer);
                    pointer += sizeof(long);
                    break;
                case DataTypeID.UInt64:
                    val_ = BitConverter.ToUInt64(data, pointer);
                    pointer += sizeof(ulong);
                    break;
                case DataTypeID.Float16:
                    val_ = BitConverter.ToHalf(data, pointer);
                    pointer += sizeof(short);
                    break;
                case DataTypeID.Float32:
                    val_ = BitConverter.ToSingle(data, pointer);
                    pointer += sizeof(float);
                    break;
                case DataTypeID.Float64:
                    val_ = BitConverter.ToInt16(data, pointer);
                    pointer += sizeof(double);
                    break;
                default:
                    throw new Exception($"Datatype {type} is not a fixed data type");
            }
            return val_;
        }
        private T ReadArray<T>(byte[] data, ref int pointer)
        {
            return (T)ReadArray(data, ref pointer, DataTypes.GetDataTypeIDFromType(typeof(T)));
        }
        private object ReadArray(byte[] data, ref int pointer, DataTypeID dtype)
        {
            if ((int)dtype < (int)DataTypeID.Int8_array ||
            (int)dtype > (int)DataTypeID.String_array)
                throw new Exception($"Datatype {dtype} is not a array type!");

            switch (dtype)
            {
                case DataTypeID.Int8_array:
                    return m_ReadFixedArray<byte>(data, ref pointer);
                case DataTypeID.SInt8_array:
                    return m_ReadFixedArray<sbyte>(data, ref pointer);
                case DataTypeID.Int16_array:
                    return m_ReadFixedArray<short>(data, ref pointer);
                case DataTypeID.UInt16_array:
                    return m_ReadFixedArray<ushort>(data, ref pointer);
                case DataTypeID.Int32_array:
                    return m_ReadFixedArray<int>(data, ref pointer);
                case DataTypeID.UInt32_array:
                    return m_ReadFixedArray<byte>(data, ref pointer);
                case DataTypeID.Int64_array:
                    return m_ReadFixedArray<long>(data, ref pointer);
                case DataTypeID.UInt64_array:
                    return m_ReadFixedArray<ulong>(data, ref pointer);
                case DataTypeID.Float16_array:
                    return m_ReadFixedArray<Half>(data, ref pointer);
                case DataTypeID.Float32_array:
                    return m_ReadFixedArray<float>(data, ref pointer);
                case DataTypeID.Float64_array:
                    return m_ReadFixedArray<double>(data, ref pointer);
                case DataTypeID.StdString:
                    return m_ReadString_std(data, ref pointer);
                case DataTypeID.String_array:
                    return m_ReadStringArray(data, ref pointer);
                default:
                    throw new Exception($"Datatype {dtype} is not a array type!");
            }
        }
        private T[] m_ReadFixedArray<T>(byte[] data, ref int pointer)
        {
            int length = ReadLength(data, ref pointer);
            T[] ret = new T[length];
            DataTypeID type = DataTypes.GetDataTypeIDFromType(typeof(T));
            for (int i = 0; i < length; i++)
            {
                ret[i] = (T)ReadFixed(data, ref pointer, type);
            }
            return ret;
        }
        private string m_ReadString_std(byte[] data, ref int pointer)
        {
            int length = ReadLength(data, ref pointer);
            string s = "";
            switch (stringEncoding)
            {
                case StringEncodingMode.UTF8:
                    s = Encoding.UTF8.GetString(data, pointer, length);
                    pointer += length;
                    break;
                case StringEncodingMode.Unicode:
                    s = Encoding.Unicode.GetString(data, pointer, length);
                    pointer += length;
                    break;
                case StringEncodingMode.ASCII:
                    s = Encoding.ASCII.GetString(data, pointer, length);
                    pointer += length;
                    break;
                default:
                    throw new Exception($"Invalid string encoding {stringEncoding}!");
            }
            return s;
        }
        private string[] m_ReadStringArray(byte[] data, ref int pointer)
        {
            int length = ReadLength(data, ref pointer);
            string[] str = new string[length];
            for (int i = 0; i < length; i++)
            {
                str[i] = m_ReadString_std(data, ref pointer);
            }
            return str;
        }
    }
}