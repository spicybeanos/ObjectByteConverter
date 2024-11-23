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

        private int WriteLength(long length, byte[] buffer, int start)
        {
            var len = EncodeLength(length);
            for (int i = 0; i < len.Length; i++)
            {
                buffer[i + start] = len[i];
            }
            return len.Length;
        }

        public byte[] EncodeValue(object value)
        {
            var type = DataType.GetType(value);

            if (type == PrimType.Object)
            {
                throw new Exception($"Cannot encode an object!");
            }

            if (type == PrimType.Array)
            {
                return EncodeArray(value);
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
            else if (type == PrimType.Null)
            {
                return new byte[] { (byte)PrimType.Null };
            }
            else
            {
                return EncodeSingleValue(value, type);
            }
        }

        private byte[] EncodeArray(object value)
        {
            var type = DataType.GetArrayType(value);
            byte[] data;
            //                [token:array]  [token:arr_type][number of elements]
            int datagramStart = sizeof(byte) + sizeof(byte) + DataType.SizeOf(lengthEncoding);
            int length;
            switch (type)
            {
                case PrimType.Bool:

                    {
                        bool[] a = (bool[])value;
                        length = a.Length;
                        data = new byte[a.Length + datagramStart];
                        for (int i = 0, j = datagramStart; i < a.Length; i++, j++)
                        {
                            data[j] = (byte)(a[i] ? 1 : 0);
                        }
                    }
                    break;
                case PrimType.UInt8:

                    {
                        byte[] a = (byte[])value;
                        length = a.Length;
                        data = new byte[a.Length + datagramStart];
                        for (int i = 0, j = datagramStart; i < a.Length; i++, j++)
                        {
                            data[j] = a[i];
                        }
                    }
                    break;
                case PrimType.Int16:

                    {
                        short[] a = (short[])value;
                        length = a.Length;
                        int size_t = sizeof(short);
                        data = new byte[datagramStart + a.Length * size_t];
                        for (int i = 0, j = datagramStart; i < a.Length; i++, j += size_t)
                        {
                            byte[] bf = BitConverter.GetBytes(a[i]);
                            data[j] = bf[0];
                            data[j + 1] = bf[1];
                        }
                    }
                    break;
                case PrimType.Int32:

                    {
                        int[] a = (int[])value;
                        length = a.Length;
                        int size_t = sizeof(int);
                        data = new byte[datagramStart + a.Length * size_t];
                        for (int i = 0, j = datagramStart; i < a.Length; i++, j += size_t)
                        {
                            byte[] bf = BitConverter.GetBytes(a[i]);
                            data[j] = bf[0];
                            data[j + 1] = bf[1];
                            data[j + 2] = bf[2];
                            data[j + 3] = bf[3];
                        }
                    }
                    break;
                case PrimType.Int64:

                    {
                        long[] a = (long[])value;
                        length = a.Length;
                        int size_t = sizeof(long);
                        data = new byte[datagramStart + a.Length * size_t];
                        for (int i = 0, j = datagramStart; i < a.Length; i++, j += size_t)
                        {
                            byte[] bf = BitConverter.GetBytes(a[i]);
                            data[j] = bf[0];
                            data[j + 1] = bf[1];
                            data[j + 2] = bf[2];
                            data[j + 3] = bf[3];
                            data[j + 4] = bf[4];
                            data[j + 5] = bf[5];
                            data[j + 6] = bf[6];
                            data[j + 7] = bf[7];
                        }
                    }
                    break;
                case PrimType.Float32:

                    {
                        float[] a = (float[])value;
                        length = a.Length;
                        int size_t = sizeof(float);
                        data = new byte[datagramStart + a.Length * size_t];
                        for (int i = 0, j = datagramStart; i < a.Length; i++, j += size_t)
                        {
                            byte[] bf = BitConverter.GetBytes(a[i]);
                            data[j] = bf[0];
                            data[j + 1] = bf[1];
                            data[j + 2] = bf[2];
                            data[j + 3] = bf[3];
                        }
                    }
                    break;
                case PrimType.Float64:

                    {
                        double[] a = (double[])value;
                        length = a.Length;
                        int size_t = sizeof(double);
                        data = new byte[datagramStart + a.Length * size_t];
                        for (int i = 0, j = datagramStart; i < a.Length; i++, j += size_t)
                        {
                            byte[] bf = BitConverter.GetBytes(a[i]);
                            data[j] = bf[0];
                            data[j + 1] = bf[1];
                            data[j + 2] = bf[2];
                            data[j + 3] = bf[3];
                            data[j + 4] = bf[4];
                            data[j + 5] = bf[5];
                            data[j + 6] = bf[6];
                            data[j + 7] = bf[7];
                        }
                    }
                    break;
                case PrimType.String:

                    {
                        string[] a = (string[])value;
                        length = a.Length;
                        int blen = 0;
                        byte[][] bf = new byte[length][];
                        for (int i = 0; i < length; i++)
                        {
                            bf[i] = EncodeString(a[i]);
                            blen += bf[i].Length;
                        }
                        data = new byte[datagramStart + blen];
                        for (int i = 0, j = datagramStart; i < length; i++)
                        {
                            Buffer.BlockCopy(bf[i], 0, data, j, bf[i].Length);
                            j += bf[i].Length;
                        }
                    }
                    break;
                default:
                    throw new Exception($"Cannot contruct this type of array : {type.ToString()}");
            }
            data[0] = (byte)PrimType.Array;
            data[1] = (byte)type;
            WriteLength(length, data, 2);
            return data;
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

        /// <summary>
        /// writes the number of bytes then wites the bytes of the string encoded
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private byte[] EncodeString(string value)
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
