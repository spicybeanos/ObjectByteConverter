using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ByteConverter
{
    public class Serializer
    {
        public byte[] Serialize(object obj, Type type)
        {
            FieldInfo[] fields = type.GetFields();
            return null;
        }
    }
}
