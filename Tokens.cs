
namespace ByteConverter
{
    public enum DefToken
    {
        //size_t
        DictClassCount = 1,
        //size t >> FieldData.GetBytes() n times
        ClassFeilds,
        //std string
        ClassFullName,
        ClassAssemblyQualifiedName,
        //int32
        ClassID,
        //std string
        FieldName,
        //DataTypesID
        FieldDataType,
        //int32
        FieldClassID,

        FieldDataEnd = 253,
        ClassDataEnd = 254,
        DictionaryEnd = 255

    }
    public enum SerializationTokens
    {
        EndOfDatagram,
        Identifier
    }
}