using System.Collections;
using System.Reflection;

namespace ByteConverter
{
    
    public class ClassDefinitions
    {
        public const int PRIMITIVE_TYPE_CLASS_ID = -1;
        private static int ClassCounter = 0;

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
        }

        /*
            while encoding, write the class name , class id , and class info
            together to take up less space
        */

        public int GetClassID(Type type)
        {
            if (ClassIDDictionary.ContainsKey(type))
                return ClassIDDictionary[type];
            if (!TryAddClass(type))
                return PRIMITIVE_TYPE_CLASS_ID;
            return ClassIDDictionary[type];
        }
        private bool TryAddClass(Type type)
        {
            try
            {
                if (DataTypes.GetDataTypeIDFromType(type) != DataTypeID.UserDefined)
                    return false;

                if (ClassIDDictionary.ContainsKey(type))
                    return false;
                
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
                    GlobalDefinitions[classID].fields[i] = FieldData.FromFieldInfo(fieldsIn[i],this);
                
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}