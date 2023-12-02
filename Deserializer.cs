using System.ComponentModel;
using System;
using System.Text;
using System.Text.Json;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
        private PrimitiveDecoder decoder { get; set; }
        private ClassDefinitions Definitions { get; set; }
        public Deserializer(byte[] data)
        {
            this.data = data;
        }
        
        public void Desialize<T>(ref T outputValue)
        {
            
        }
        public object DeserializeValue(ref int pointer)
        {
            DataTypeID typeID = (DataTypeID)data[pointer++];
            switch (typeID)
            {
                case DataTypeID.Null: return null;
                case >= DataTypeID.Char and <= DataTypeID.String_array:
                    return decoder.DecodePrimitive(data, ref pointer, typeID);
                default: throw new NotImplementedException("Not implimented object deserialization");
            }
        }
        public object DeserializeObject(ref int pointer)
        {
            int classID = decoder.DecodeSizeT(data,ref pointer);
            if (classID == ClassDefinitions.NULL_VALUE_CLASS_ID) return null;

            throw new NotImplementedException();
        }
    }
}

