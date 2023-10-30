namespace ByteConverter
{
    public enum DataTypesID
    {
        I8,
        S8,
        I16,
        U16,
        I32,
        U32,
        I64,
        U64,
        F32,
        F64,
        Size_T,
        I8_array,
        S8_array,
        I16_array,
        U16_array,
        I32_array,
        U32_array,
        I64_array,
        U64_array,
        F32_array,
        F64_array,
        Char,
        String,
        String_array,
        //unicode string
        UString,
        Object,
    }
    public class DataTypes
    {
        private static readonly Dictionary<Type, DataTypesID>
        _type_dataTypeID = new Dictionary<Type, DataTypesID>()
        {
            {typeof(byte),DataTypesID.I8},
            {typeof(short),DataTypesID.I16},
            {typeof(int),DataTypesID.I32},
            {typeof(long),DataTypesID.I64},
            {typeof(sbyte),DataTypesID.S8},
            {typeof(ushort),DataTypesID.U16},
            {typeof(uint),DataTypesID.U32},
            {typeof(ulong),DataTypesID.U64},
            {typeof(float),DataTypesID.F32},
            {typeof(double),DataTypesID.F64},
            {typeof(char),DataTypesID.Char},
            {typeof(string),DataTypesID.String},

            {typeof(byte[]),DataTypesID.I8_array},
            {typeof(short[]),DataTypesID.I16_array},
            {typeof(int[]),DataTypesID.I32_array},
            {typeof(long[]),DataTypesID.I64_array},
            {typeof(sbyte[]),DataTypesID.S8_array},
            {typeof(ushort[]),DataTypesID.U16_array},
            {typeof(uint[]),DataTypesID.U32_array},
            {typeof(ulong[]),DataTypesID.U64_array},
            {typeof(float[]),DataTypesID.F32_array},
            {typeof(double[]),DataTypesID.F64_array},
            {typeof(char[]),DataTypesID.String},
            {typeof(string[]),DataTypesID.String_array}
        };
        public static DataTypesID TypeToDataTypeID(Type type)
        {
            if (_type_dataTypeID.ContainsKey(type))
                return _type_dataTypeID[type];
            return DataTypesID.Object;
        }
    }
}