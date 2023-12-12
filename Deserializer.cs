using System.ComponentModel;
using System;
using System.Text;
using System.Text.Json;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Collections;
/*
 * not null
 (token)identifier (string)name (datatypeid)type value
 (token)identifier (string)name (datatypeid)[userdefined] (size_t)classID {object serilization}

 * null
 (token)identifier (string)name (datatypeid)[null]
 (token)identifier (string)name (datatypeid)[userdefined] (size_t)[null]

 
*/
namespace ByteConverter
{
    public class Deserializer
    {
        private byte[] data { get; set; }
        private PrimitiveDecoder Decoder { get; set; }
        private MetaInf metaInf;
        public ClassDefinitions Definitions { get; set; }
        public Deserializer(byte[] data)
        {
            this.data = data;

        }
        public void DeserializeMeta(ref int pointer)
        {
            metaInf = MetaInfReader.ReadMetaInf(data, ref pointer);
            Decoder = new PrimitiveDecoder(metaInf);
        }
        public void DeserializeClassDefinations(ref int pointer)
        {
            Definitions = ClassDefinitions.FromBytes(data, ref pointer, Decoder);
        }
        public object Deserialize()
        {
            int ptr = 0;
            DeserializeMeta(ref ptr);
            DeserializeClassDefinations(ref ptr);
            return DeserializeValue(ref ptr);
        }
        public object DeserializeValue(ref int pointer)
        {
            DataTypeID typeID = (DataTypeID)data[pointer++];
            switch (typeID)
            {
                case DataTypeID.Null: 
                    return null;
                case >= DataTypeID.Char and <= DataTypeID.String_array:
                    return Decoder.DecodePrimitive(data, ref pointer, typeID);
                case DataTypeID.UserDefined: 
                    return DeserializeObject(ref pointer);
                case DataTypeID.UserDefinedArray:
                    return DeserializeObjectArray(ref pointer);
                default:
                    throw new Exception($"Cannot deserialize data type '{typeID}'");
            }
        }
        public object DeserializeObjectArray(ref int pointer)
        {
            int length = Decoder.DecodeSizeT(data,ref pointer);
            object el0 = DeserializeObject(ref pointer);
            IList arr = (IList)Activator.CreateInstance(el0.GetType().MakeArrayType());
            arr.Add(el0);
            for (int i = 1; i < length; i++)
            {
                arr.Add(DeserializeObject(ref pointer));
            }
            return arr;
        }
        public object DeserializeObject(ref int pointer)
        {
            int classID = Decoder.DecodeSizeT(data, ref pointer);
            if (classID == ClassDefinitions.NULL_VALUE_CLASS_ID) return null;
            ClassData cdata = Definitions.GetClassData(classID);
            Type ctype = Type.GetType(cdata.ClassFullName);
            FieldInfo[] finfo = ctype.GetFields();
            Dictionary<string, object> varObjVal = new();
            object cobj;
            try
            {
                cobj = Activator.CreateInstance(ctype);
            }
            catch (System.MissingMethodException ex)
            {
                throw new Exception($"Make sure all objects invloved have atleast one parameterless consturctor!\nClass : {cdata.ClassFullName}\n" + ex);
            }

            if (cobj == null)
                throw new Exception($"Activator object is null! Aborting deserialization!");

            int nosFields = cdata.fields.Length;
            for (int i = 0; i < nosFields; i++)
            {
                DataTypeID typeID = (DataTypeID)data[pointer++];
                string fname = cdata.fields[i].FieldName;
                if (DataTypes.IsPrimitive( typeID))
                {
                    var value = Decoder.DecodePrimitive(data, ref pointer, typeID);
                    Console.WriteLine(fname + ":" + value);
                    varObjVal.Add(fname, value);
                }
                else if (typeID == DataTypeID.UserDefined)
                {
                    var value = DeserializeObject(ref pointer);
                    varObjVal.Add(fname, value);
                }
                else if(typeID == DataTypeID.UserDefinedArray)
                {
                    var val = DeserializeObjectArray(ref pointer);
                    varObjVal.Add(fname,val);
                }
                else{
                    throw new Exception($"Unexpected type \'{typeID}\' for field '{cdata.ClassFullName}.{fname}'");
                }
            }
            foreach (var field in finfo)
            {
                if (!varObjVal.ContainsKey(field.Name))
                    continue;
                field.SetValue(cobj, varObjVal[field.Name]);
                Console.WriteLine(field.Name + ":" + field.GetValue(cobj));
            }
            return cobj;
        }
    }
}

