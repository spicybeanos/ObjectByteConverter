using System.Text;

namespace ByteConverter
{
    public enum MetaInfTokens
    {
        MetaInfEnd,
        AssemblyQName,
        Size_TReader,
        StringEncoding,
        ClassName,
        Length
    }
    public class MetaInf
    {
        public string ClassName { get; set; }
        public DataTypeIDs SizeTReader { get; set; }
        public StringEncodingMode stringEncodingMode { get; set; }
        public bool IncludeAssemblyQualifiedName { get; set; } = false;
        public int Length { get; set; }
        public MetaInf(string className, DataTypeIDs sizeTReader,
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
             metaInf.SizeTReader, metaInf.Length, metaInf.stringEncodingMode,metaInf.IncludeAssemblyQualifiedName);
        }
        public static byte[] GenerateMetaInfBytes
        (string className,
        DataTypeIDs sizeTReader, int Length,
        StringEncodingMode stringEncodingMode, bool includeAssemblyQualifiedName)
        {
            List<byte> ret = new()
            {
                (byte)MetaInfTokens.Size_TReader,
                (byte)sizeTReader,
                (byte)MetaInfTokens.AssemblyQName,
                BoolToByte(includeAssemblyQualifiedName),
                (byte)MetaInfTokens.ClassName
            };
            ret.AddRange(MStringToBytes(className));
            ret.Add((byte)MetaInfTokens.Length);
            ret.AddRange(Int32Writer(Length));
            ret.Add((byte)MetaInfTokens.StringEncoding);
            ret.Add((byte)stringEncodingMode);
            ret.Add((byte)MetaInfTokens.MetaInfEnd);
            return ret.ToArray();
        }
        private static byte BoolToByte(bool value)
        {
            return (byte)(value ? 1 : 0);
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
            MetaInfTokens tok;
            MetaInf inf = new();
            do
            {
                tok = (MetaInfTokens)buffer.Dequeue();
                switch (tok)
                {
                    case MetaInfTokens.Size_TReader:
                        {
                            DataTypeIDs sizeT = (DataTypeIDs)buffer.Dequeue();
                            inf.SizeTReader = sizeT;
                        }
                        break;
                    case MetaInfTokens.Length:
                        {
                            int len = ReadInt();
                            inf.Length = len;
                        }
                        break;
                    case MetaInfTokens.ClassName:
                        {
                            string className = ReadMString();
                            inf.ClassName = className;
                        }
                        break;
                    case MetaInfTokens.StringEncoding:
                        {
                            StringEncodingMode mode = (StringEncodingMode)buffer.Dequeue();
                            inf.stringEncodingMode = mode;
                            break;
                        }
                    case MetaInfTokens.AssemblyQName:
                        {
                            bool asm = ReadBool();
                            inf.IncludeAssemblyQualifiedName = asm;
                        }
                        break;
                    case MetaInfTokens.MetaInfEnd:
                        break;
                    default:
                        throw new Exception($"Invalid MetaInfToken {tok}");
                }
            }
            while (tok != MetaInfTokens.MetaInfEnd);
            return inf;
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