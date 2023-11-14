using System.Collections;
using System.Reflection;

namespace ByteConverter
{
    public enum ClassDictionaryTokens
    {
        //size_t
        Dict_NumberOfClasses = 1,
        //size t
        ClassFeilds,
        //std string
        ClassFullName,
        ClassAssemblyQualifiedName,
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

        FieldDataEnd = 253,
        ClassDataEnd = 254,
        DictionaryEnd = 255

    }
    public class UserDefinitions
    {
        public const int PRIMITIVE_TYPE_CLASS_ID = -1;

        // class name to class id
        public Dictionary<Type, int> ClassIDDictionary
        { get; set; } = new Dictionary<Type, int>();

        //class id to class info
        public Dictionary<int, ClassInfo> GlobalDefinitions
        { get; set; } = new Dictionary<int, ClassInfo>();

        /*
            while encoding, write the class name , class id , and class info
            together to take up less space
            type id >> type info
        */

        public int GetClassID(Type type)
        {
            if (ClassIDDictionary.ContainsKey(type))
                return ClassIDDictionary[type];
            if (!AddClass(type))
                return PRIMITIVE_TYPE_CLASS_ID;
            return ClassIDDictionary[type];
        }
        public bool AddClass(Type type)
        {
            try
            {
                Random r = new Random();
                FieldInfo[] fieldsIn = type.GetFields();
                Console.WriteLine($"Field length : {fieldsIn.Length}");


                string className = type.FullName, assembly = type.Assembly.FullName;
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
                    ClassFullName = className,
                    ClassID = classID,
                    ClassAssemblyQualifiedName = assembly,
                    fields = new FieldData[fieldsIn.Length]
                };
                //populate the field data
                for (int i = 0; i < fieldsIn.Length; i++)
                {
                    classData.fields[i] = new()
                    {
                        FieldName = fieldsIn[i].Name,
                        fieldDataType = DataTypes.GetDataTypeIDFromType(fieldsIn[i].FieldType)
                    };
                    Console.WriteLine(fieldsIn[i].Name + "\t" + classData.fields[i].fieldDataType);
                    classData.fields[i].FieldClassID =
                    classData.fields[i].fieldDataType == DataTypeIDs.UserDefined ?
                    GetClassID(fieldsIn[i].FieldType)
                    : PRIMITIVE_TYPE_CLASS_ID;
                }
                Console.WriteLine($"fields size : {classData.fields.Length}");
                ClassIDDictionary.Add(type, classID);
                GlobalDefinitions.Add(classID, classData);
                Console.WriteLine($"dict field size: {GlobalDefinitions[classID].fields.Length}");

                return true;
            }
            catch
            {
                return false;
            }
        }
        public byte[] GenerateTypesDictionaryByte(StandardEncoder encoder)
        {
            List<byte> ret = new()
            {
                (byte)ClassDictionaryTokens.Dict_NumberOfClasses
            };
            int nos_classes = ClassIDDictionary.Count();
            ret.AddRange(encoder.EncodeSizeT(nos_classes));
            if (ClassIDDictionary.Count() != GlobalDefinitions.Count())
                throw new Exception($"{nameof(ClassIDDictionary)}'s count does not equal {nameof(GlobalDefinitions)}'s count!");

            foreach (Type type in ClassIDDictionary.Keys)
            {
                int id_ = GetClassID(type);
                ClassInfo info = ClassInfo.FromType(type, this);

                ret.AddRange(encoder.EncodePrimitive(id_));
                ret.AddRange(info.GenerateBytes(encoder));
            }
            ret.Add((byte)ClassDictionaryTokens.DictionaryEnd);

            return ret.ToArray();
        }
        public static UserDefinitions FromBytes(byte[] data, ref int pointer, StandardDecoder decoder)
        {
            UserDefinitions ud = new();
            ClassDictionaryTokens tok;
            do
            {
                tok = (ClassDictionaryTokens)data[pointer++];
                switch (tok)
                {
                    case ClassDictionaryTokens.Dict_NumberOfClasses:
                        int nos_classes = decoder.ReadSizeT(data, ref pointer);
                        for (int i = 0; i < nos_classes; i++)
                        {
                            int tId = decoder.ReadFixed<int>(data, ref pointer);
                            ClassInfo info =
                             ClassInfo.FromBytes(data, decoder, ref pointer);
                            Type type = Type.GetType(info.ClassFullName) ?? Type.GetType(info.ClassAssemblyQualifiedName);
                            ud.ClassIDDictionary.Add(type, tId);
                            ud.GlobalDefinitions.Add(tId, info);
                        }
                        break;
                }
            }
            while (tok != ClassDictionaryTokens.DictionaryEnd);
            return ud;
        }
    }
    public class ClassInfo
    {
        public string ClassFullName { get; set; }
        public string ClassAssemblyQualifiedName { get; set; }
        public int ClassID { get; set; }
        public FieldData[] fields { get; set; } = new FieldData[0];
        public byte[] GenerateBytes(StandardEncoder encoder)
        {
            List<byte> ret = new(){
                (byte)ClassDictionaryTokens.ClassID
            };
            ret.AddRange(encoder.EncodePrimitive(ClassID));
            ret.Add((byte)ClassDictionaryTokens.ClassFullName);
            ret.AddRange(encoder.EncodePrimitive(ClassFullName));
            //ret.Add((byte)ClassDictionaryTokens.ClassAssemblyQualifiedName);
            //ret.AddRange(encoder.EncodePrimitive(ClassAssemblyQualifiedName));
            ret.Add((byte)ClassDictionaryTokens.ClassFeilds);
            Console.WriteLine($"size in to bytes : {fields.Length}");
            ret.AddRange(encoder.EncodeSizeT(fields.Length));
            for (int i = 0; i < fields.Length; i++)
            {
                ret.AddRange(fields[i].GenerateBytes(encoder));
            }

            ret.Add((byte)ClassDictionaryTokens.ClassDataEnd);
            return ret.ToArray();
        }
        public static ClassInfo FromType<T>(UserDefinitions definitions)
        {
            return FromType(typeof(T), definitions);
        }
        public static ClassInfo FromType(Type type, UserDefinitions definitions)
        {
            ClassInfo ci = new()
            {
                ClassFullName = type.FullName,
                ClassAssemblyQualifiedName = type.AssemblyQualifiedName,
                ClassID = definitions.GetClassID(type)
            };
            return ci;
        }
        public static ClassInfo FromBytes(byte[] data, StandardDecoder decoder, ref int pointer)
        {
            ClassInfo ci = new();
            ClassDictionaryTokens tok;
            do
            {
                tok = (ClassDictionaryTokens)data[pointer++];
                switch (tok)
                {
                    case ClassDictionaryTokens.ClassFullName:
                        ci.ClassFullName = decoder.DecodePrimitive<string>(data, ref pointer);
                        break;
                    case ClassDictionaryTokens.ClassAssemblyQualifiedName:
                        ci.ClassAssemblyQualifiedName = decoder.DecodePrimitive<string>(data, ref pointer);
                        break;
                    case ClassDictionaryTokens.ClassID:
                        ci.ClassID = decoder.DecodePrimitive<int>(data, ref pointer);
                        break;
                    case ClassDictionaryTokens.ClassFeilds:
                        {
                            int length = decoder.ReadSizeT(data, ref pointer);
                            ci.fields = new FieldData[length];
                            for (int i = 0; i < length; i++)
                            {
                                ci.fields[i] = FieldData.FromBytes(data, decoder, ref pointer);
                            }
                        }
                        break;
                }
            }
            while (tok != ClassDictionaryTokens.ClassDataEnd);
            return ci;
        }

        public override string ToString()
        {
            string res = "{\n";
            res += $"{nameof(ClassFullName)}:{ClassFullName},\n";
            res += $"{nameof(ClassID)}:{ClassID},\n";
            res += $"{nameof(ClassAssemblyQualifiedName)}:{ClassAssemblyQualifiedName},\n";
            if (fields != null)
            {
                res += $"{nameof(fields)}({fields.Length}):[\n";
                foreach (var f in fields)
                {
                    res += f.ToString() + ";\n";
                }
                res += "]";
            }
            res += "}";
            return res;
        }
    }
    public class FieldData
    {
        public DataTypeIDs fieldDataType { get; set; }
        public int FieldClassID { get; set; } = UserDefinitions.PRIMITIVE_TYPE_CLASS_ID;
        public string FieldName { get; set; }

        public byte[] GenerateBytes(StandardEncoder encoder)
        {
            List<byte> ret = new(){
                (byte)ClassDictionaryTokens.FieldDataType,
                (byte)fieldDataType,
                (byte)ClassDictionaryTokens.FieldName
            };
            ret.AddRange(encoder.EncodePrimitive(FieldName));
            if (FieldClassID >= 0)
            {
                ret.Add((byte)ClassDictionaryTokens.FieldClassID);
                ret.AddRange(encoder.EncodePrimitive(FieldClassID));
            }
            ret.Add((byte)ClassDictionaryTokens.FieldDataEnd);
            return ret.ToArray();
        }
        public static FieldData FromField(FieldInfo fieldInfo)
        {
            FieldData fd = new()
            {
                FieldName = fieldInfo.Name,
                fieldDataType = DataTypes.GetDataTypeIDFromType(fieldInfo.FieldType)
            };
            return fd;
        }
        public static FieldData FromBytes(byte[] data, StandardDecoder decoder, ref int pointer)
        {
            FieldData fd = new();
            ClassDictionaryTokens tok;
            do
            {
                tok = (ClassDictionaryTokens)data[pointer++];
                switch (tok)
                {
                    case ClassDictionaryTokens.FieldDataType:
                        {
                            fd.fieldDataType = (DataTypeIDs)data[pointer++];
                        }
                        break;
                    case ClassDictionaryTokens.FieldName:
                        {
                            fd.FieldName = decoder.ReadArray<string>(data, ref pointer);
                        }
                        break;
                    case ClassDictionaryTokens.FieldClassID:
                        {
                            fd.FieldClassID = decoder.ReadFixed<int>(data, ref pointer);
                        }
                        break;
                }
            }
            while (tok != ClassDictionaryTokens.FieldDataEnd);
            return fd;
        }

        public override string ToString()
        {
            return $"{{\n{nameof(fieldDataType)}:{fieldDataType},\n{nameof(FieldName)}:{FieldName},\n{nameof(FieldClassID)}:{FieldClassID}\n}}";
        }
    }
}