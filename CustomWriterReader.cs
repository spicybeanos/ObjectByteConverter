


namespace ByteConverter
{

    public interface CustomSerializer
    {
        public byte[] WriteObject(object obj, PrimitiveEncoder encoder, ClassDefinitions definitions);
        public object ReadObject(byte[] data, ref int pointer, PrimitiveDecoder decoder, ClassDefinitions definitions);
    }
}