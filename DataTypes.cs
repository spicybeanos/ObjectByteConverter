namespace ByteConverter
{
    public enum DataTypeIDs
    {
        //put any fixed type between char and float64
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
        String_std,
        String_ascii,
        String_array,
        //unicode string

        UserDefined,
        DynamicArray

    }
    public class DataTypes
    {
        private static readonly Dictionary<Type, DataTypeIDs>
        _type_dataTypeID = new Dictionary<Type, DataTypeIDs>()
        {

            {typeof(byte),DataTypeIDs.Int8},
            {typeof(short),DataTypeIDs.Int16},
            {typeof(int),DataTypeIDs.Int32},
            {typeof(long),DataTypeIDs.Int64},
            {typeof(sbyte),DataTypeIDs.SInt8},
            {typeof(ushort),DataTypeIDs.UInt16},
            {typeof(uint),DataTypeIDs.UInt32},
            {typeof(ulong),DataTypeIDs.UInt64},
            {typeof(float),DataTypeIDs.Float32},
            {typeof(Half),DataTypeIDs.Float16},
            {typeof(double),DataTypeIDs.Float64},
            {typeof(char),DataTypeIDs.Char},
            {typeof(bool),DataTypeIDs.Boolean},
            {typeof(string),DataTypeIDs.String_std},

            {typeof(byte[]),DataTypeIDs.Int8_array},
            {typeof(short[]),DataTypeIDs.Int16_array},
            {typeof(int[]),DataTypeIDs.Int32_array},
            {typeof(long[]),DataTypeIDs.Int64_array},
            {typeof(sbyte[]),DataTypeIDs.SInt8_array},
            {typeof(ushort[]),DataTypeIDs.UInt16_array},
            {typeof(uint[]),DataTypeIDs.UInt32_array},
            {typeof(ulong[]),DataTypeIDs.UInt64_array},
            {typeof(Half[]),DataTypeIDs.Float16_array},
            {typeof(float[]),DataTypeIDs.Float32_array},
            {typeof(double[]),DataTypeIDs.Float64_array},
            {typeof(char[]),DataTypeIDs.String_std},
            {typeof(string[]),DataTypeIDs.String_array}
        };
        public static DataTypeIDs GetDataTypeIDFromType(Type type)
        {
            if (_type_dataTypeID.ContainsKey(type))
                return _type_dataTypeID[type];
            return DataTypeIDs.UserDefined;
        }
        public static bool IsFixed(DataTypeIDs type)
        {
            return (int)type >= (int)DataTypeIDs.Char && (int)type <= (int)DataTypeIDs.Float64;
        }
        public static bool IsArray(DataTypeIDs type)
        {
            return (int)type >= (int)DataTypeIDs.Int8_array && (int) type <= (int)DataTypeIDs.String_array;
        }
    }
}