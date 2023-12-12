using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
/*
    * not null
    (datatypeid)[userdefined] (size_t)classID {object serilization}
    (datatypeid)primitive_type (serialize)value

    * user defined array
    array elements must all be the same type!!!
    (datatypeid)[userdefined_array] (size_t)length (size_t)classID {object serilization} ,(size_t)classID {object serilization} ,(size_t)classID {object serilization}...n times

    * null
    (token)identifier (string)name (datatypeid)[null]

    *not null inside a user defined type object serilization
    [(datatypeid)type] (serialize)value
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
        public ClassDefinitions Definitions { get; set; }
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
            Result r = Definitions.TryAddClass(value.GetType());
            if(r.Success)
                return ClassDefinitions.GetBytes(Definitions, Encoder);
            else
                throw new Exception($"Failed to add type : {r.exception}");
        }
        private byte[] SeriliarizeBody(object value)
        {
            List<byte> body = new List<byte>();
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
            if (value.GetType().IsArray)
            {
                IEnumerable? vals = value as IEnumerable;
                if (vals == null)
                    return new byte[] { (byte)DataTypeID.Null };

                int ctr = 0;
                foreach (var _ in vals)
                {
                    ctr++;
                }
                data.AddRange(Encoder.EncodeSizeT(ctr));
                foreach (var item in vals)
                {
                    data.AddRange(SerializeUserDefinedValue(item));
                }
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
