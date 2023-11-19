// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text;
using ByteConverter;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        MetaInf metaInf = new()
        {
            stringEncodingMode = StringEncodingMode.UTF8,
            SizeTReader = DataTypeID.UInt16
        };
        StandardEncoder encoder = new(metaInf);
        StandardDecoder decoder = new(metaInf);
        int ptr = 0;
        JsonSerializerOptions op = new() { WriteIndented = true };
        ClassDefinitions definitions = new();

        definitions.TryAddClass(typeof(Transform));
        Console.WriteLine(definitions.DEBUG());
        var bts = ClassDefinitions.GetBytes(definitions,encoder);
        Console.WriteLine(ByteArrayToString(bts));
        var classDef = ClassDefinitions.FromBytes(bts,ref ptr,decoder);
        Console.WriteLine(classDef.DEBUG());
    }
    private static string ByteArrayToString(byte[] arrInput)
    {
        int i;
        StringBuilder sOutput = new StringBuilder(arrInput.Length);
        for (i = 0; i < arrInput.Length; i++)
        {
            sOutput.Append(arrInput[i].ToString("x2"));
            sOutput.Append(" ");
        }
        return sOutput.ToString();
    }
}


class Transform
{
    public string name = "";
    public Quaternion rotation = Quaternion.Identity;
    public Vector3 position = Vector3.zero;
    public Transform transform = null;
}
class Quaternion
{
    public float x = 0, y = 0, z = 0, w = 0;
    public static Quaternion Identity { get { return new Quaternion() { x = 0, y = 0, z = 0, w = 0 }; } }
}
class Vector3
{
    public Transform transform = null;
    public float x = 0, y = 0, z = 0;
    public static Vector3 zero { get { return new Vector3() { x = 0, y = 0, z = 0 }; } }
}