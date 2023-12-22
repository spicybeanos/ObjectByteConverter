


namespace ByteConverter
{
    public class CustomWriterReader
    {
        public Type type { get; private set; }
        public virtual byte[] WriteObject(object obj, PrimitiveEncoder encoder,ClassDefinitions definitions)
        {
            return null;
        }
        public virtual object ReadObject(byte[] data, ref int pointer, PrimitiveDecoder decoder, ClassDefinitions definitions)
        {
            return null;
        }
    }
}