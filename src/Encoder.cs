using System.Text;
using System.Text.Encodings;

namespace ByteConverter
{
    /// <summary>
    /// Encodes the type and the binary value
    /// [type] [binary value]
    /// OR
    /// [type (array?)] [array type] [length (n) number of elements] [binary value of all the values in the array]
    /// [type (string?)] [length (n) number of bytes] [binary]
    /// </summary>
    public class Encoder
    {
        public PrimType lengthEncoding { get; private set; }
        public StringEncoding stringEncoding { get; private set; }

        public Encoder(PrimType lengthType, StringEncoding stringEncoding)
        {
            this.lengthEncoding = lengthType;
            this.stringEncoding = stringEncoding;
        }

        private byte[] EncodeLength(long length)
        {
            switch (lengthEncoding)
            {
                case PrimType.UInt8:
                    return new byte[] { (byte)length };
                case PrimType.Int16:
                    return BitConverter.GetBytes((short)length);
                case PrimType.Int32:
                    return BitConverter.GetBytes((int)length);
                case PrimType.Int64:
                    return BitConverter.GetBytes(length);
                default:
                    throw new Exception(
                        $"Invalid length encoding type! `{lengthEncoding.ToString()}`"
                    );
            }
        }

        public byte[] EncodeValue(object value)
        {
            var type = DataType.GetType(value);
            if (type == PrimType.Object)
            {
                throw new Exception($"Cannot encode an object!");
            }

            if (type == PrimType.Array) {

            }

            if (type == PrimType.String)
            {
                byte[] data = EncodeString((string)value);
                byte[] ret = new byte[data.Length + DataType.SizeOf(PrimType.Type)];
                ret[0] = (byte)PrimType.String;
                Buffer.BlockCopy(data, 0, ret, 1, data.Length);
                data = null;
                return ret;
            }
        }

        private byte[] EncodeArray(object value){
            var type = DataType.GetArrayType(value);
            
        }

        private byte[] EncodeSingleValue(object value, PrimType type)
        {
            switch (type)
            {
                case PrimType.Bool:
                    return new byte[] { (byte)((bool)value ? 1 : 0) };
                case PrimType.UInt8:
                    return new byte[] { (byte)value };
                case PrimType.Int16:
                    return BitConverter.GetBytes((short)value);
                case PrimType.Int32:
                    return BitConverter.GetBytes((int)value);
                case PrimType.Int64:
                    return BitConverter.GetBytes((long)value);
                case PrimType.Float32:
                    return BitConverter.GetBytes((float)value);
                case PrimType.Float64:
                    return BitConverter.GetBytes((double)value);
                default:
                    throw new Exception($"invalid type: `{type}`. not constant size type");
            }
        }

        public byte[] EncodeString(string value)
        {
            byte[] str;

            switch (stringEncoding)
            {
                case StringEncoding.UTF8:
                    str = Encoding.UTF8.GetBytes(value);
                    break;
                case StringEncoding.ASCII:
                    str = Encoding.ASCII.GetBytes(value);
                    break;
                case StringEncoding.Unicode:
                    str = Encoding.Unicode.GetBytes(value);
                    break;
                default:
                    throw new Exception($"Invalid string encoding `{stringEncoding.ToString()}` !");
            }
            byte[] len = EncodeLength(str.Length);
            byte[] data = new byte[str.Length + DataType.SizeOf(lengthEncoding)];
            Buffer.BlockCopy(len, 0, data, 0, DataType.SizeOf(lengthEncoding));
            Buffer.BlockCopy(str, 0, data, DataType.SizeOf(lengthEncoding), str.Length);
            str = null;
            len = null;
            return data;
        }
    }
}
