using System.Reflection;

namespace ByteConverter
{
    public class FieldData
    {
        public DataTypeID FieldDataType { get; set; }
        //to be encoded as size T
        public int FieldClassID { get; set; } = ClassDefinitions.PRIMITIVE_TYPE_CLASS_ID;
        public string FieldName { get; set; }


        public static FieldData FromFieldInfo(FieldInfo info, ClassDefinitions classDefinitions)
        {
            return new FieldData()
            {
                FieldName = info.Name,
                FieldDataType = DataTypes.GetDataTypeIDFromType(info.FieldType),
                FieldClassID = classDefinitions.GetClassID(info.FieldType)
            };
        }
        public static byte[] GetBytes(FieldData fieldData, StandardEncoder encoder)
        {
            List<byte> ret = new()
            {
                (byte)DefToken.FieldDataType,
                (byte)fieldData.FieldDataType
            };
            if (fieldData.FieldClassID > -1)
            {
                ret.Add((byte)DefToken.FieldClassID);
                ret.AddRange(encoder.EncodeSizeT(fieldData.FieldClassID));
            }
            ret.Add((byte)DefToken.FieldName);
            ret.AddRange(encoder.EncodePrimitive(fieldData.FieldName));
            ret.Add((byte)DefToken.FieldDataEnd);
            byte[] bytes = ret.ToArray();
            return bytes;
        }
        public static FieldData FromBytes(byte[] data, ref int pointer, StandardDecoder decoder)
        {
            FieldData fieldData = new();
            DefToken tok;
            do
            {
                tok = (DefToken)data[pointer++];
                switch (tok)
                {
                    case DefToken.FieldDataType:
                        DataTypeID dt = (DataTypeID)data[pointer++];
                        fieldData.FieldDataType = dt;
                        break;
                    case DefToken.FieldName:
                        string fname = decoder.DecodePrimitive<string>(data, ref pointer);
                        fieldData.FieldName = fname;
                        break;
                    case DefToken.FieldClassID:
                        int id = decoder.DecodeSizeT(data, ref pointer);
                        fieldData.FieldClassID = id;
                        break;
                }
            } while (tok != DefToken.FieldDataEnd);
            return fieldData;
        }
    }
}