
namespace ByteConverter
{
    public class AnyNDArray
    {
        /*
        (dtype)data_type (size_t)rank , (rank x size_t)lengths 

        */
        public static List<byte> SerializeNDArray(object array,PrimitiveEncoder encoder,Serializer serializer)
        {
            List<byte> result = new List<byte>();
            Type ty = array.GetType();
            var dt = ty.GetElementType();
            if (ty.IsSZArray)
            {
                throw new Exception("Is not N-Dimentional! Use the normal serializer!");
            }
            if (!ty.IsArray)
            {
                throw new Exception("Is not an array! Use the nnormal serializer!");
            }
            int rank = ty.GetArrayRank();
            int[] lengths = new int[rank];
            for (int i = 0;i < rank;i++){
                lengths[i] = ((Array)array).GetLength(i);
            }
            
            result.Add((byte)DataTypes.GetDataTypeIDFromType(dt));
            result.AddRange(encoder.EncodeSizeT(rank));
            for (int i = 0; i < rank; i++)
            {
                result.AddRange(encoder.EncodeSizeT(lengths[i]));
            }

            

            return null;
        }
    }
}

