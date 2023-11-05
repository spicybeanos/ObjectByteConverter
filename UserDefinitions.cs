using System.Reflection;

namespace ByteConverter
{
    public enum UserDefinitionTokens
    {
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
        FieldClassID

    }
    public class UserDefinitions
    {
        public const int NOT_USER_DEFINED = -1;

        public static Dictionary<Type, int>
        ClassIDDictionary
        { get; set; } = new Dictionary<Type, int>();

        public static Dictionary<int, ClassInfo>
        GlobalPrototypes
        { get; set; } = new Dictionary<int, ClassInfo>();

        public static int HandleUserDefinedFielType(Type type)
        {
            if (ClassIDDictionary.ContainsKey(type))
                return ClassIDDictionary[type];
            if (!AddClass(type))
                return NOT_USER_DEFINED;
            return ClassIDDictionary[type];
        }

        public static bool AddClass(Type type)
        {
            try
            {
                Random r = new Random();
                FieldInfo[] fields = type.GetFields();
                FieldData[] fieldData = new FieldData[fields.Length];

                string className = type.ToString();
                int classID = r.Next();

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
                        fieldDataType = DataTypes.TypeToDataTypeID(fields[i].FieldType)
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
    }
    public class ClassInfo
    {
        public string ClassName { get; set; }
        public int ClassID { get; set; }
        public FieldData[] fields { get; set; }
    }
    public class FieldData
    {
        public DataTypeIDs fieldDataType { get; set; }
        public int FieldClassID { get; set; } = UserDefinitions.NOT_USER_DEFINED;
        public string FieldName { get; set; }
    }
}