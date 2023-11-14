using System.Text;

namespace ByteConverter
{
    public class StandardDecoder
    {
        public StringEncodingMode stringEncoding { get; private set; }
        public DataTypeIDs sizeT { get; private set; }
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
        public StandardDecoder(DataTypeIDs size_t, StringEncodingMode stringEncodingMode)
        {
            stringEncoding = stringEncodingMode;
            sizeT = size_t;
        }
        public StandardDecoder(MetaInf inf)
        {
            sizeT = inf.SizeTReader;
            stringEncoding = inf.stringEncodingMode;
        }
        public object DecodePrimitive(byte[] data, ref int pointer, DataTypeIDs type)
        {
            if (DataTypes.IsFixed(type))
                return ReadFixed(data, ref pointer, type);
            else if (DataTypes.IsArray(type))
                return ReadArray(data, ref pointer, type);
            else
                throw new Exception($"{type} is not a primitive type!");
        }
        public T DecodePrimitive<T>(byte[] data, ref int pointer)
        {
            return (T)DecodePrimitive(data, ref pointer, DataTypes.GetDataTypeIDFromType(typeof(T)));
        }
        public int ReadSizeT(byte[] data, ref int pointer){
            return ReadLength(data,ref pointer);
        }
        private int ReadLength(byte[] data, ref int pointer)
        {
            long val_ = 0;
            switch (sizeT)
            {
                case DataTypeIDs.Int8:
                    val_ = data[pointer];
                    pointer += sizeof(byte);
                    break;
                case DataTypeIDs.SInt8:
                    val_ = data[pointer];
                    pointer += sizeof(sbyte);
                    break;
                case DataTypeIDs.Int16:
                    val_ = BitConverter.ToInt16(data, pointer);
                    pointer += sizeof(short);
                    break;
                case DataTypeIDs.UInt16:
                    val_ = BitConverter.ToUInt16(data, pointer);
                    pointer += sizeof(ushort);
                    break;
                case DataTypeIDs.Int32:
                    val_ = BitConverter.ToInt32(data, pointer);
                    pointer += sizeof(int);
                    break;
                case DataTypeIDs.UInt32:
                    val_ = BitConverter.ToUInt32(data, pointer);
                    pointer += sizeof(uint);
                    break;
                case DataTypeIDs.Int64:
                    val_ = BitConverter.ToInt64(data, pointer);
                    pointer += sizeof(long);
                    break;
                case DataTypeIDs.UInt64:
                    val_ = (long)BitConverter.ToUInt64(data, pointer);
                    pointer += sizeof(ulong);
                    break;
                default:
                    throw new Exception($"Datatype {sizeT} is not a valid length encoding!");
            }
            return (int)val_;
        }
        public T ReadFixed<T>(byte[] data, ref int pointer)
        {
            return (T)ReadFixed(data, ref pointer, DataTypes.GetDataTypeIDFromType(typeof(T)));
        }
        public object ReadFixed(byte[] data, ref int pointer, DataTypeIDs type)
        {
            if (!DataTypes.IsFixed(type))
                throw new Exception($"Datatype {type} is not a fixed type!");
            object val_;
            switch (type)
            {
                case DataTypeIDs.Char:
                    val_ = BitConverter.ToChar(data, pointer);
                    pointer += sizeof(char);
                    break;
                case DataTypeIDs.Boolean:
                    val_ = BitConverter.ToBoolean(data, pointer);
                    pointer += sizeof(bool);
                    break;
                case DataTypeIDs.Int8:
                    val_ = data[pointer];
                    pointer += sizeof(byte);
                    break;
                case DataTypeIDs.SInt8:
                    val_ = data[pointer];
                    pointer += sizeof(sbyte);
                    break;
                case DataTypeIDs.Int16:
                    val_ = BitConverter.ToInt16(data, pointer);
                    pointer += sizeof(short);
                    break;
                case DataTypeIDs.UInt16:
                    val_ = BitConverter.ToUInt16(data, pointer);
                    pointer += sizeof(ushort);
                    break;
                case DataTypeIDs.Int32:
                    val_ = BitConverter.ToInt32(data, pointer);
                    pointer += sizeof(int);
                    break;
                case DataTypeIDs.UInt32:
                    val_ = BitConverter.ToUInt32(data, pointer);
                    pointer += sizeof(uint);
                    break;
                case DataTypeIDs.Int64:
                    val_ = BitConverter.ToInt64(data, pointer);
                    pointer += sizeof(long);
                    break;
                case DataTypeIDs.UInt64:
                    val_ = BitConverter.ToUInt64(data, pointer);
                    pointer += sizeof(ulong);
                    break;
                case DataTypeIDs.Float16:
                    val_ = BitConverter.ToHalf(data, pointer);
                    pointer += sizeof(short);
                    break;
                case DataTypeIDs.Float32:
                    val_ = BitConverter.ToSingle(data, pointer);
                    pointer += sizeof(float);
                    break;
                case DataTypeIDs.Float64:
                    val_ = BitConverter.ToInt16(data, pointer);
                    pointer += sizeof(double);
                    break;
                default:
                    throw new Exception($"Datatype {type} is not a fixed data type");
            }
            return val_;
        }
        public T ReadArray<T>(byte[] data, ref int pointer)
        {
            return (T)ReadArray(data, ref pointer, DataTypes.GetDataTypeIDFromType(typeof(T)));
        }
        public object ReadArray(byte[] data, ref int pointer, DataTypeIDs dtype)
        {
            if ((int)dtype < (int)DataTypeIDs.Int8_array ||
            (int)dtype > (int)DataTypeIDs.String_array)
                throw new Exception($"Datatype {dtype} is not a array type!");

            switch (dtype)
            {
                case DataTypeIDs.Int8_array:
                    return m_ReadFixedArray<byte>(data, ref pointer);
                case DataTypeIDs.SInt8_array:
                    return m_ReadFixedArray<sbyte>(data, ref pointer);
                case DataTypeIDs.Int16_array:
                    return m_ReadFixedArray<short>(data, ref pointer);
                case DataTypeIDs.UInt16_array:
                    return m_ReadFixedArray<ushort>(data, ref pointer);
                case DataTypeIDs.Int32_array:
                    return m_ReadFixedArray<int>(data, ref pointer);
                case DataTypeIDs.UInt32_array:
                    return m_ReadFixedArray<byte>(data, ref pointer);
                case DataTypeIDs.Int64_array:
                    return m_ReadFixedArray<long>(data, ref pointer);
                case DataTypeIDs.UInt64_array:
                    return m_ReadFixedArray<ulong>(data, ref pointer);
                case DataTypeIDs.Float16_array:
                    return m_ReadFixedArray<Half>(data, ref pointer);
                case DataTypeIDs.Float32_array:
                    return m_ReadFixedArray<float>(data, ref pointer);
                case DataTypeIDs.Float64_array:
                    return m_ReadFixedArray<double>(data, ref pointer);
                case DataTypeIDs.String_std:
                    return m_ReadString_std(data, ref pointer);
                case DataTypeIDs.String_array:
                    return m_ReadStringArray(data, ref pointer);
                default:
                    throw new Exception($"Datatype {dtype} is not a array type!");
            }
        }
        private T[] m_ReadFixedArray<T>(byte[] data, ref int pointer)
        {
            int length = ReadLength(data, ref pointer);
            T[] ret = new T[length];
            DataTypeIDs type = DataTypes.GetDataTypeIDFromType(typeof(T));
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