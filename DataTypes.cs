using System.Collections;

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
        Float32,
        Float64,

        Size_T,

        //dynamic starts from int8_array and ends at String_array
        Int8_array,
        SInt8_array,
        Int16_array,
        UInt16_array,
        Int32_array,
        UInt32_array,
        Int64_array,
        UInt64_array,
        Float32_array,
        Float64_array,
        StdString,
        String_ascii,
        String_array,
        //unicode string

        UserDefined,
        UserDefinedArray,

        CustomType
    }
    public enum StringEncodingMode
    {
        UTF8,
        Unicode,
        ASCII
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
            {typeof(float[]),DataTypeID.Float32_array},
            {typeof(double[]),DataTypeID.Float64_array},
            {typeof(char[]),DataTypeID.StdString},
            {typeof(string[]),DataTypeID.String_array}
        };

        /// <summary>
        /// Gets the size of the data type of fixed length.
        /// Null has a size of 0.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static int SizeOf(DataTypeID type)
        {
            return type switch
            {
                DataTypeID.Null => 0,
                DataTypeID.Char => sizeof(char),
                DataTypeID.Boolean => sizeof(bool),
                DataTypeID.Int8 => sizeof(byte),
                DataTypeID.SInt8 => sizeof(sbyte),
                DataTypeID.Int16 => sizeof(short),
                DataTypeID.UInt16 => sizeof(ushort),
                DataTypeID.Int32 => sizeof(int),
                DataTypeID.UInt32 => sizeof(uint),
                DataTypeID.Int64 => sizeof(long),
                DataTypeID.UInt64 => sizeof(ulong),
                DataTypeID.Float32 => sizeof(float),
                DataTypeID.Float64 => sizeof(double),
                _ => throw new NotSupportedException($"type {type} does not have a fixed size!")
            };
        }
        public static DataTypeID GetDataTypeIDFromType(Type type)
        {
            if (_type_dataTypeID.ContainsKey(type))
                return _type_dataTypeID[type];
            if(type.IsArray)
                return DataTypeID.UserDefinedArray;
            return DataTypeID.UserDefined;
        }
        public static bool IsFixed(DataTypeID type)
        {
            return (int)type >= (int)DataTypeID.Char && (int)type <= (int)DataTypeID.Float64;
        }
        public static bool IsArray(Type type)
        {
            return type.GetInterfaces().Contains(typeof(IEnumerable));
        }
        public static bool IsPrimitiveArray(DataTypeID type)
        {
            return (int)type >= (int)DataTypeID.Int8_array && (int)type <= (int)DataTypeID.String_array;
        }
        public static bool IsPrimitive(DataTypeID type)
        {
            return (int)type >= (int)DataTypeID.Null && (int)type <= (int)DataTypeID.String_array;
        }
        public static bool IsUserDefined(DataTypeID type){
            return type == DataTypeID.UserDefined || type == DataTypeID.UserDefinedArray;
        }
    }
}