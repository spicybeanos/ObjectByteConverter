using System.Text;

namespace ByteConverter
{
    public enum MetaInfToken
    {

        Size_TReader = 1,
        StringEncoding,
        ClassName,
        Length,
        MetaInfEnd = 252,
    }
    public class MetaInf
    {
        public string ClassName { get; set; }
        public DataTypeID SizeTReader { get; set; }
        public StringEncodingMode stringEncodingMode { get; set; }
        public int Length { get; set; } = 0;
        public MetaInf(string className, DataTypeID sizeTReader,
         int length, StringEncodingMode stringEncoding)
        {
            ClassName = className;
            SizeTReader = sizeTReader;
            stringEncodingMode = stringEncoding;
            Length = length;
        }
        public MetaInf() { }
    }
    public class MetaInfWriter
    {
        public static byte[] GenerateMetaInfBytes(MetaInf metaInf)
        {
            return GenerateMetaInfBytes(metaInf.ClassName,
             metaInf.SizeTReader, metaInf.Length, metaInf.stringEncodingMode);
        }
        public static int LengthOfMetaInf(MetaInf metaInf)
        {
            return GenerateMetaInfBytes(metaInf).Length;
        }
        public static byte[] GenerateMetaInfBytes
        (string className,
        DataTypeID sizeTReader, int Length,
        StringEncodingMode stringEncodingMode)
        {
            List<byte> ret = new()
            {
                (byte)MetaInfToken.Size_TReader,
                (byte)sizeTReader,
                (byte)MetaInfToken.ClassName
            };
            ret.AddRange(MStringToBytes(className));
            ret.Add((byte)MetaInfToken.Length);
            ret.AddRange(Int32Writer(Length));
            ret.Add((byte)MetaInfToken.StringEncoding);
            ret.Add((byte)stringEncodingMode);
            ret.Add((byte)MetaInfToken.MetaInfEnd);
            return ret.ToArray();
        }
        private static byte[] MStringToBytes(string value)
        {
            List<byte> ret = new();
            ret.AddRange(Encoding.ASCII.GetBytes(value));
            ret.Add(0);
            return ret.ToArray();
        }
        private static byte[] Int32Writer(int val)
        {
            List<byte> ret = new();
            ret.AddRange(BitConverter.GetBytes(val));
            return ret.ToArray();
        }
    }
    public class MetaInfReader
    {
        Queue<byte> buffer { get; set; }
        public MetaInfReader(byte[] data)
        {
            buffer = new Queue<byte>(data);
        }
        public MetaInf Read()
        {
            MetaInfToken tok;
            MetaInf inf = new();
            do
            {
                tok = (MetaInfToken)buffer.Dequeue();
                switch (tok)
                {
                    case MetaInfToken.Size_TReader:
                        {
                            DataTypeID sizeT = (DataTypeID)buffer.Dequeue();
                            inf.SizeTReader = sizeT;
                        }
                        break;
                    case MetaInfToken.Length:
                        {
                            int len = ReadInt();
                            inf.Length = len;
                        }
                        break;
                    case MetaInfToken.ClassName:
                        {
                            string className = ReadMString();
                            inf.ClassName = className;
                        }
                        break;
                    case MetaInfToken.StringEncoding:
                        {
                            StringEncodingMode mode = (StringEncodingMode)buffer.Dequeue();
                            inf.stringEncodingMode = mode;
                            break;
                        }
                    case MetaInfToken.MetaInfEnd:
                        break;
                    default:
                        throw new Exception($"Invalid MetaInfToken {tok}");
                }
            }
            while (tok != MetaInfToken.MetaInfEnd);
            return inf;
        }
        public static MetaInf ReadMetaInf(byte[] data, ref int pointer)
        {
            MetaInfToken tok;
            MetaInf meta = new MetaInf();
            do
            {
                tok = (MetaInfToken)data[pointer++];
                switch (tok)
                {
                    case MetaInfToken.Size_TReader:
                        {
                            DataTypeID dt = (DataTypeID)data[pointer++];
                            meta.SizeTReader = dt;
                        }
                        break;
                    case MetaInfToken.Length:
                        {
                            int len = MReadInt(data, ref pointer);
                            meta.Length = len;
                        }
                        break;
                    case MetaInfToken.ClassName:
                        {
                            string cname = MReadString(data, ref pointer);
                            meta.ClassName = cname;
                        }
                        break;
                    case MetaInfToken.StringEncoding:
                        {
                            StringEncodingMode smode = (StringEncodingMode)data[pointer++];
                            meta.stringEncodingMode = smode;
                        }
                        break;
                    case MetaInfToken.MetaInfEnd:break;
                    default:
                        throw new Exception($"Invalid MetaInfToken {tok}");
                }
            }
            while (tok != MetaInfToken.MetaInfEnd);
            return meta;
        }

        private static int MReadInt(byte[] data, ref int pointer)
        {
            int val = BitConverter.ToInt32(data, pointer);
            pointer += sizeof(int);
            return val;
        }
        private static string MReadString(byte[] data, ref int pointer)
        {
            List<byte> buf = new();
            byte b = data[pointer++];
            do
            {
                buf.Add(b);
                b = data[pointer++];
            } while (b != 0);
            return Encoding.ASCII.GetString(buf.ToArray());
        }

        private bool ReadBool()
        {
            byte b = buffer.Dequeue();
            return b switch
            {
                1 => true,
                0 => false,
                _ => throw new Exception($"Incorrect boolean value {b}")
            };
        }
        private int ReadInt()
        {
            byte[] buf = new byte[4];
            for (int i = 0; i < 4; i++)
                buf[i] = buffer.Dequeue();
            return BitConverter.ToInt32(buf);
        }
        private string ReadMString()
        {
            List<byte> buf = new();
            byte b = buffer.Dequeue();
            while (b != 0)
            {
                buf.Add(b);
                b = buffer.Dequeue();
            }
            return Encoding.ASCII.GetString(buf.ToArray());
        }
    }

}