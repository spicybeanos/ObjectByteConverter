using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
/*
    * not null
    (token)identifier (string)name (datatypeid)[userdefined] (size_t)classID {object serilization}
    (token)identifier (string)name (datatypeid)type value

    * null
    token)identifier (string)name (datatypeid)[null]

    *not null inside a user defined type object serilization
    [(datatypeid)type][value]
    [DataTypeID.UserDefined][(size_t)classID]{serialized}
    
    *null inside a user defined type object serilization
    [DataTypeID.Null = 0]
    [DataTypeID.Null]
    [0]
 
*/
namespace ByteConverter
{
    public class Serializer
    {
        private PrimitiveEncoder Encoder { get; set; }
        private ClassDefinitions Definitions { get; set; }
        private MetaInf metaInf { get; set; }

        public Serializer(MetaInf metaInf)
        {
            Encoder = new PrimitiveEncoder(metaInf);
            Definitions = new ClassDefinitions();
            this.metaInf = metaInf;
        }

        public byte[] Serialize(object value)
        {
            List<byte> data = new();
            metaInf.ClassName = value.GetType().FullName;
            var meta = MetaInfWriter.GenerateMetaInfBytes(metaInf);
            var def = SerializeTypeDictionary(value);
            var body = SeriliarizeBody(value);
            int length = meta.Length + def.Length + body.Length;
            metaInf.Length = length;
            meta = MetaInfWriter.GenerateMetaInfBytes(metaInf);

            data.AddRange(meta);
            data.AddRange(def);
            data.AddRange(body);

            return data.ToArray();
        }
        private byte[] SerializeTypeDictionary(object value)
        {
            Definitions.TryAddClass(value.GetType());
            return ClassDefinitions.GetBytes(Definitions, Encoder);
        }
        private byte[] SeriliarizeBody(object value)
        {
            List<byte> body = new List<byte>();
            Definitions.GetClassID(value.GetType());
            metaInf.ClassName = value.GetType().FullName;
            body.AddRange(SerializeValue(value));
            return body.ToArray();
        }
        public byte[] GetMetaInfBytes()
        {
            return MetaInfWriter.GenerateMetaInfBytes(metaInf);
        }
#nullable enable


        private byte[] SerializeValue(object? value)
        {
            if (value == null)
                return new byte[] { (byte)DataTypeID.Null };
            Type type = value.GetType();
            DataTypeID typeID = DataTypes.GetDataTypeIDFromType(type);
            List<byte> data = new List<byte>()
            {
                (byte)typeID
            };
            if (DataTypes.IsPrimitive(typeID))
            {
                data.AddRange(Encoder.EncodePrimitive(value, typeID));
                return data.ToArray();
            }
            //handle user defined types
            data.AddRange(SerializeUserDefinedValue(value));
            return data.ToArray();
        }
        private byte[] SerializeUserDefinedValue(object? value)
        {
            if (value == null)
            {
                List<byte> d = new List<byte>();
                d.AddRange(Encoder.EncodeSizeT(ClassDefinitions.NULL_VALUE_CLASS_ID));
                return d.ToArray();
            }

            Type type = value.GetType();
            int classID = Definitions.GetClassID(type);
            var fields = type.GetFields();

            List<byte> data = new();
            data.AddRange(Encoder.EncodeSizeT(classID));

            ClassData classData = Definitions.GetClassData(classID);
            for (int i = 0; i < classData.fields.Length; i++)
            {
                if (classData.fields[i].FieldName != fields[i].Name)
                    continue;
                object? val = fields[i].GetValue(value);
                data.AddRange(SerializeValue(val));
            }

            return data.ToArray();
        }
    }
}
