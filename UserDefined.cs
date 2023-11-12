using System.Reflection;

namespace ByteConverter
{
    public enum UserDefinitionTokens
    {
        //size_t
        NumberOfClasses,
        Feilds,
        //std string
        ClassName,
        //int32
        ClassID,
        //size t
        ClassFieldLength,
        //std string
        FieldName,
        //DataTypesID
        FieldDataType,
        //int32
        FieldClassID,

        FieldDataEnd,
        ClassDataEnd,
        DictionaryEnd

    }
    public class UserDefined
    {
        public const int NOT_USER_DEFINED = -1;

        public Dictionary<Type, int> ClassIDDictionary
        { get; set; } = new Dictionary<Type, int>();

        public Dictionary<int, ClassInfo> GlobalPrototypes
        { get; set; } = new Dictionary<int, ClassInfo>();

        public int HandleUserDefinedFielType(Type type)
        {
            if (ClassIDDictionary.ContainsKey(type))
                return ClassIDDictionary[type];
            if (!AddClass(type))
                return NOT_USER_DEFINED;
            return ClassIDDictionary[type];
        }
        public bool AddClass(Type type)
        {
            try
            {
                Random r = new Random();
                FieldInfo[] fields = type.GetFields();
                FieldData[] fieldData = new FieldData[fields.Length];

                string className = type.ToString();
                int classID = r.Next();

                if (DataTypes.GetDataTypeIDFromType(type) != DataTypeIDs.UserDefined)
                    return false;

                if (ClassIDDictionary.ContainsValue(classID))
                    classID = r.Next();

                //check if the class has already been listed
                if (ClassIDDictionary.ContainsKey(type))
                    return false;

                ClassInfo classData = new()
                {
                    ClassName = className,
                    ClassID = classID
                };
                for (int i = 0; i < fields.Length; i++)
                {
                    fieldData[i] = new()
                    {
                        FieldName = fields[i].Name,
                        fieldDataType = DataTypes.GetDataTypeIDFromType(fields[i].FieldType)
                    };
                    fieldData[i].FieldClassID =
                    fieldData[i].fieldDataType == DataTypeIDs.UserDefined ?
                    HandleUserDefinedFielType(fields[i].FieldType)
                    : NOT_USER_DEFINED;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        public byte[] GenerateTypesDictionary(MetaInf metaInf)
        {
            List<byte> ret = new();
            StandardEncoder encoder = new(metaInf.SizeTReader, metaInf.stringEncodingMode);
            ret.Add((byte)UserDefinitionTokens.NumberOfClasses);
            int nos_classes = ClassIDDictionary.Count();
            ret.AddRange(encoder.EncodeSizeT(nos_classes));



            return ret.ToArray();
        }

    }
    public class ClassInfo
    {
        public string ClassName { get; set; }
        public int ClassID { get; set; }
        public FieldData[] fields { get; set; }
        public byte[] GenerateBytes(StandardEncoder encoder)
        {
            List<byte> ret = new(){
                (byte)UserDefinitionTokens.ClassName
            };
            ret.AddRange(encoder.EncodePrimitive(ClassName));
            ret.Add((byte)UserDefinitionTokens.ClassID);
            ret.AddRange(encoder.EncodePrimitive(ClassID));
            ret.Add((byte)UserDefinitionTokens.Feilds);
            ret.AddRange(encoder.EncodeSizeT(fields.Length));
            for (int i = 0; i < fields.Length; i++)
            {
                ret.AddRange(fields[i].GenerateBytes(encoder));
            }
            return ret.ToArray();
        }
    }
    public class FieldData
    {
        public DataTypeIDs fieldDataType { get; set; }
        public int FieldClassID { get; set; } = UserDefined.NOT_USER_DEFINED;
        public string FieldName { get; set; }

        public byte[] GenerateBytes(StandardEncoder encoder)
        {
            List<byte> ret = new(){
                (byte)UserDefinitionTokens.FieldDataType,
                (byte)fieldDataType,
                (byte)UserDefinitionTokens.FieldName
            };
            ret.AddRange(encoder.EncodePrimitive(FieldName));
            if (FieldClassID > 0)
            {
                ret.Add((byte)UserDefinitionTokens.FieldClassID);
                ret.AddRange(encoder.EncodePrimitive(FieldClassID));
            }
            ret.Add((byte)UserDefinitionTokens.FieldDataEnd);
            return ret.ToArray();
        }
        public static FieldData FromBytes(byte[] data, MetaInf inf)
        {
            FieldData fd = new();
            int ptr = 0;
            StandardDecoder decoder = new(inf);
            UserDefinitionTokens tok;
            do
            {
                tok = (UserDefinitionTokens)data[ptr++];
                switch (tok)
                {
                    case UserDefinitionTokens.FieldDataType:
                        {
                            fd.fieldDataType = (DataTypeIDs)data[ptr++];
                        }
                        break;
                    case UserDefinitionTokens.FieldName:
                        {
                            fd.FieldName = decoder.ReadArray<string>(data, ref ptr);
                        }
                        break;
                    case UserDefinitionTokens.FieldClassID:
                        {
                            fd.FieldClassID = decoder.ReadFixed<int>(data,ref ptr);
                        }break;
                }
            }
            while (tok != UserDefinitionTokens.FieldDataEnd);
            return fd;
        }
    }
}