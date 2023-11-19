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



        var bytes1 = ClassData.GetBytes(
                    ClassData.FromType(typeof(Transform), definitions),
                    encoder
                );
        var bytes2 = ClassData.GetBytes(
            ClassData.FromType(typeof(Vector3), definitions), encoder
        );
        var bytes3 = ClassData.GetBytes(
            ClassData.FromType(typeof(Quaternion), definitions), encoder
        );
        Console.WriteLine(
            ByteArrayToString(
                bytes1
            )
            );
        Console.WriteLine();
        Console.WriteLine(
        ByteArrayToString(
            bytes2
        )
        );
        Console.WriteLine();
        Console.WriteLine(
        ByteArrayToString(
            bytes3
        )
        );
        ClassData classData = ClassData.FromBytes(bytes1, ref ptr, decoder);
        Console.WriteLine(
            JsonSerializer.Serialize(classData,op)
        );

        Console.WriteLine();
        ptr = 0;
        classData = ClassData.FromBytes(bytes2, ref ptr, decoder);
        Console.WriteLine(
            JsonSerializer.Serialize(classData,op)
        );
        
        Console.WriteLine();
        ptr = 0;
        classData = ClassData.FromBytes(bytes3, ref ptr, decoder);
        Console.WriteLine(
            JsonSerializer.Serialize(classData,op)
        );
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
    public Transform transform;
}
class Quaternion
{
    public float x = 0, y = 0, z = 0, w = 0;
    public static Quaternion Identity { get { return new Quaternion() { x = 0, y = 0, z = 0, w = 0 }; } }
}
class Vector3
{
    public Transform transform;
    public float x = 0, y = 0, z = 0;
    public static Vector3 zero { get { return new Vector3() { x = 0, y = 0, z = 0 }; } }
}