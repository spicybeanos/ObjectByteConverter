using System.Collections;
using System.Reflection;

namespace ByteConverter
{

    public class ClassDefinitions
    {
        public const int PRIMITIVE_TYPE_CLASS_ID = -1;
        public const int NULL_VALUE_CLASS_ID = 0;
        private int ClassCounter { get; set; } = NULL_VALUE_CLASS_ID + 1;

        // class name to class id
        private Dictionary<Type, int> ClassIDDictionary
        { get; set; }

        //class id to class info
        private Dictionary<int, ClassData> GlobalDefinitions
        { get; set; }

        public ClassDefinitions()
        {
            ClassIDDictionary = new Dictionary<Type, int>();
            GlobalDefinitions = new Dictionary<int, ClassData>();
            ClassCounter = NULL_VALUE_CLASS_ID + 1;
        }

        /*
            while encoding, write the class name , class id , and class info
            together to take up less space
        */
        public string DEBUG()
        {
            string s = nameof(ClassIDDictionary) + ":[\n";
            foreach (var type in ClassIDDictionary.Keys)
            {
                s += $"\t{{name={type.FullName}:clsID={ClassIDDictionary[type]}}},\n";
            }
            s += "]\n" + nameof(GlobalDefinitions) + ":[\n";
            foreach (var clsID in GlobalDefinitions.Keys)
            {
                s += $"\t{{clsID={clsID}:fields={GlobalDefinitions[clsID].fields.Length}}},\n";
            }
            s += "]\n";
            return s;
        }
        public static byte[] GetBytes(ClassDefinitions definitions, PrimitiveEncoder encoder)
        {
            List<byte> ret = new()
            {
                (byte)DefToken.DictClassCount
            };
            ret.AddRange(encoder.EncodeSizeT(definitions.GlobalDefinitions.Count()));
            foreach (var cl in definitions.GlobalDefinitions.Values)
            {
                ret.AddRange(ClassData.GetBytes(cl, encoder));
            }
            ret.Add((byte)DefToken.DictionaryEnd);
            return ret.ToArray();
        }
        public static ClassDefinitions FromBytes(byte[] data, ref int pointer, PrimitiveDecoder decoder)
        {
            DefToken tok;
            ClassDefinitions definitions = new();
            do
            {
                tok = (DefToken)data[pointer++];
                switch (tok)
                {
                    case DefToken.DictClassCount:
                        int count = decoder.DecodeSizeT(data, ref pointer);
                        for (int i = 0; i < count; i++)
                        {
                            var cls = ClassData.FromBytes(data, ref pointer, decoder);
                            Type type = Type.GetType(cls.ClassFullName);
                            definitions.ClassIDDictionary.Add(type, cls.ClassID);
                            definitions.GlobalDefinitions.Add(cls.ClassID, cls);
                        }

                        break;
                }
            }
            while (tok != DefToken.DictionaryEnd);
            return definitions;
        }
        public int GetClassID(Type type)
        {
            if (ClassIDDictionary.ContainsKey(type))
                return ClassIDDictionary[type];
            if (type.IsArray)
                return GetClassID(type.GetElementType());
            Result r = TryAddClass(type);
            if (!r.Success)
            {
                if (r.exception is NotUserDefinedException)
                    return PRIMITIVE_TYPE_CLASS_ID;
                else
                    throw new CouldNotDefineTypeException(type, r.exception);
            }
            return ClassIDDictionary[type];
        }
        public ClassData GetClassData(int classID)
        {
            if (GlobalDefinitions.ContainsKey(classID))
                return GlobalDefinitions[classID];
            else
                throw new Exception($"A class defination for class id '{classID}' does not exist!");
        }
        public Result TryAddClass(Type type)
        {
            try
            {
                DataTypeID dt = DataTypes.GetDataTypeIDFromType(type);
                if (!DataTypes.IsUserDefined(dt))
                    return Result.Failed(new NotUserDefinedException(type));

                if (ClassIDDictionary.ContainsKey(type))
                    return Result.Successful();

                if (dt == DataTypeID.UserDefined)
                {
                    FieldInfo[] fieldsIn = type.GetFields();
                    string className = type.FullName;
                    int classID = ClassCounter++;

                    //check if the class has already been listed


                    ClassData classData = new()
                    {
                        ClassFullName = className,
                        ClassID = classID,
                        fields = new FieldData[fieldsIn.Length]
                    };

                    ClassIDDictionary.Add(type, classID);
                    GlobalDefinitions.Add(classID, classData);

                    //populate the field data
                    for (int i = 0; i < fieldsIn.Length; i++)
                    {
                        GlobalDefinitions[classID].fields[i] = FieldData.FromFieldInfo(fieldsIn[i], this);
                    }


                    return Result.Successful();
                }
                else
                {
                    Type et = type.GetElementType();
                    return TryAddClass(et);
                }

            }
            catch (Exception ex)
            {
                return Result.Failed(ex);
            }
        }
    }
}