using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text;
using ByteConverter;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;

class Program
{
    static void Main(string[] args)
    {
        MetaInf metaInf = new()
        {
            stringEncodingMode = StringEncodingMode.UTF8,
            SizeTReader = DataTypeID.UInt16
        };
        PrimitiveEncoder encoder = new(metaInf);
        PrimitiveDecoder decoder = new(metaInf);
        JsonSerializerOptions op = new() { WriteIndented = true };
        op.IncludeFields = true;
        ClassDefinitions definitions = new();

        Transform t = new(){position = new(1,3,4),rotation=new(2,43,4,-2),name="amongus"};

        Serializer ser = new(metaInf);

        var json = JsonSerializer.Serialize(t,op);
        var bts = ser.Serialize(t);
        Console.WriteLine($"serialized object: \n{ByteArrayToString(bts)}");
        Console.WriteLine($"Json :\n{json}");
        Console.WriteLine($"byte\t|\tjson\n{bts.Length}\t|\t{json.Length}");

        Deserializer des = new(bts);
        var obj = des.Deserialize();

        Console.WriteLine(JsonSerializer.Serialize(obj,op));
    }
    private static string ByteArrayToString(byte[] arrInput, bool hex = true)
    {
        int i;
        StringBuilder sOutput = new StringBuilder(arrInput.Length);
        for (i = 0; i < arrInput.Length; i++)
        {
            sOutput.Append(hex ? arrInput[i].ToString("x2") : arrInput[i]);
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
    public bool ValueEquality(Transform t)
    {
        return name == t.name && rotation.ValueEquality(t.rotation) && position.ValueEquality(t.position);
    }
}
class Quaternion
{
    public float x = 0, y = 0, z = 0, w = 0;
    public Quaternion(float x, float y, float z, float w)
    {
        this.x = x; this.y = y; this.z = z; this.w = w;
    }
    public Quaternion()
    {

    }
    public bool ValueEquality(Quaternion q)
    {
        return x == q.x && y == q.y && z == q.z && w == q.w;
    }
    private static readonly Quaternion iden = new Quaternion(0, 0, 0, 0);
    public static Quaternion Identity { get { return iden; } }
}
class Vector3
{
    public float x = 0, y = 0, z = 0;
    public Vector3()
    {

    }
    public bool ValueEquality(Vector3 v)
    {
        return x == v.x && y == v.y && z == v.z;
    }
    public Vector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    private static Vector3 zeroV = new Vector3(0, 0, 0);
    public static Vector3 zero { get { return zeroV; } }
}
class Vector3Int
{
    public int x = 0, y = 0, z = 0;
    public Vector3Int(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    private static readonly Vector3Int zerV = new Vector3Int(0, 0, 0);
    public static Vector3Int zero { get { return zerV; } }
}