namespace ByteConverter
{
    public enum StringEncoding
    {
        UTF8 = 1,
        ASCII,
        Unicode
    }

    public enum PrimType : byte
    {
        Null = 0,
        Type,
        Object,
        Bool,
        UInt8,
        Int16,
        Int32,
        Int64,
        Float32,
        Float64,
        Array,
        String
    }

    public class DataType
    {
        public static PrimType GetType(object obj)
        {
            return obj switch
            {
                null => PrimType.Null,
                bool => PrimType.Bool,
                byte => PrimType.UInt8,
                short => PrimType.Int16,
                int => PrimType.Int32,
                long => PrimType.Int64,
                float => PrimType.Float32,
                double => PrimType.Float64,
                string => PrimType.String,

                bool[] => PrimType.Array,
                byte[] => PrimType.Array,
                short[] => PrimType.Array,
                int[] => PrimType.Array,
                long[] => PrimType.Array,
                float[] => PrimType.Array,
                double[] => PrimType.Array,
                string[] => PrimType.Array,
                _ => PrimType.Object
            };
        }

        public static PrimType GetArrayType(object obj)
        {
            return obj switch
            {
                null => PrimType.Null,
                bool[] => PrimType.Bool,
                byte[] => PrimType.UInt8,
                short[] => PrimType.Int16,
                int[] => PrimType.Int32,
                long[] => PrimType.Int64,
                float[] => PrimType.Float32,
                double[] => PrimType.Float64,
                string[] => PrimType.String,
                _ => PrimType.Object
            };
        }

        public static int SizeOf(PrimType type)
        {
            return type switch
            {
                PrimType.Null => 0,
                PrimType.Bool => 1,
                PrimType.Type => 1,
                PrimType.UInt8 => 1,
                PrimType.Int16 => 2,
                PrimType.Int32 => 4,
                PrimType.Int64 => 8,
                PrimType.Float32 => 4,
                PrimType.Float64 => 8,
                _ => throw new Exception($"Cannot tell size of variable size type!")
            };
        }
    }
}
