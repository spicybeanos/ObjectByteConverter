namespace ByteConverter
{
    public enum DataTypeID
    {
        //put any fixed type between char and float64
        Null,
        Char,
        Boolean,
        Int8,
        SInt8,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Int64,
        UInt64,
        Float16,
        Float32,
        Float64,

        Size_T,

        //dynamic starts from int8_array and ends at 
        Int8_array,
        SInt8_array,
        Int16_array,
        UInt16_array,
        Int32_array,
        UInt32_array,
        Int64_array,
        UInt64_array,
        Float16_array,
        Float32_array,
        Float64_array,
        //utf-8 and length of size_t
        StdString,
        String_ascii,
        String_array,
        //unicode string

        UserDefined,
        DynamicArray

    }
    public class DataTypes
    {
        private static readonly Dictionary<Type, DataTypeID>
        _type_dataTypeID = new Dictionary<Type, DataTypeID>()
        {

            {typeof(byte),DataTypeID.Int8},
            {typeof(short),DataTypeID.Int16},
            {typeof(int),DataTypeID.Int32},
            {typeof(long),DataTypeID.Int64},
            {typeof(sbyte),DataTypeID.SInt8},
            {typeof(ushort),DataTypeID.UInt16},
            {typeof(uint),DataTypeID.UInt32},
            {typeof(ulong),DataTypeID.UInt64},
            {typeof(float),DataTypeID.Float32},
            {typeof(Half),DataTypeID.Float16},
            {typeof(double),DataTypeID.Float64},
            {typeof(char),DataTypeID.Char},
            {typeof(bool),DataTypeID.Boolean},
            {typeof(string),DataTypeID.StdString},

            {typeof(byte[]),DataTypeID.Int8_array},
            {typeof(short[]),DataTypeID.Int16_array},
            {typeof(int[]),DataTypeID.Int32_array},
            {typeof(long[]),DataTypeID.Int64_array},
            {typeof(sbyte[]),DataTypeID.SInt8_array},
            {typeof(ushort[]),DataTypeID.UInt16_array},
            {typeof(uint[]),DataTypeID.UInt32_array},
            {typeof(ulong[]),DataTypeID.UInt64_array},
            {typeof(Half[]),DataTypeID.Float16_array},
            {typeof(float[]),DataTypeID.Float32_array},
            {typeof(double[]),DataTypeID.Float64_array},
            {typeof(char[]),DataTypeID.StdString},
            {typeof(string[]),DataTypeID.String_array}
        };
        public static DataTypeID GetDataTypeIDFromType(Type type)
        {
            if (_type_dataTypeID.ContainsKey(type))
                return _type_dataTypeID[type];
            return DataTypeID.UserDefined;
        }
        public static bool IsFixed(DataTypeID type)
        {
            return (int)type >= (int)DataTypeID.Char && (int)type <= (int)DataTypeID.Float64;
        }
        public static bool IsArray(DataTypeID type)
        {
            return (int)type >= (int)DataTypeID.Int8_array && (int) type <= (int)DataTypeID.String_array;
        }
    }
}