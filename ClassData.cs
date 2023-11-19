using System.Reflection;
using System.Text;

namespace ByteConverter
{
    public class ClassData
    {
        public string ClassFullName { get; set; }
        //to be stored as sizeT
        public int ClassID { get; set; }
        public FieldData[] fields { get; set; }

        public static byte[] GetBytes(ClassData classData, StandardEncoder encoder)
        {
            List<byte> ret = new(){
                (byte)DefToken.ClassID
            };
            ret.AddRange(encoder.EncodeSizeT(classData.ClassID));
            ret.Add((byte)DefToken.ClassFullName);
            ret.AddRange(encoder.EncodePrimitive(classData.ClassFullName));
            ret.Add((byte)DefToken.ClassFeilds);
            ret.AddRange(encoder.EncodeSizeT(classData.fields.Length));
            for (int i = 0; i < classData.fields.Length; i++)
            {
                ret.AddRange(FieldData.GetBytes(classData.fields[i], encoder));
            }
            ret.Add((byte)DefToken.ClassDataEnd);
            return ret.ToArray();
        }
        public static ClassData FromBytes(byte[] data, ref int pointer, StandardDecoder decoder)
        {
            ClassData classData = new();
            DefToken token;
            do
            {
                token = (DefToken)data[pointer++];
                switch (token)
                {
                    case DefToken.ClassFullName:
                        string className = decoder.DecodePrimitive<string>(data, ref pointer);
                        classData.ClassFullName = className;
                        break;
                    case DefToken.ClassID:
                        int id = decoder.DecodeSizeT(data, ref pointer);
                        classData.ClassID = id;
                        break;
                    case DefToken.ClassFeilds:
                        int length = decoder.DecodeSizeT(data, ref pointer);
                        classData.fields = new FieldData[length];
                        for (int i = 0; i < length; i++)
                            classData.fields[i] = FieldData.FromBytes(data, ref pointer, decoder);
                        break;
                }
            } while (token != DefToken.ClassDataEnd);
            return classData;
        }
        public static ClassData FromType(Type type, ClassDefinitions definitions)
        {
            ClassData classData = new()
            {
                ClassFullName = type.FullName,
                ClassID = definitions.GetClassID(type),
            };
            FieldInfo[] fieldInfos = type.GetFields();
            classData.fields = new FieldData[fieldInfos.Length];

            for (int i = 0; i < fieldInfos.Length; i++)
                classData.fields[i] = FieldData.FromFieldInfo(fieldInfos[i], definitions);

            return classData;
        }
    }
}