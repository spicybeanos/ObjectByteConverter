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
        //int ptr = 0;
        JsonSerializerOptions op = new() { WriteIndented = true };
        op.IncludeFields = true;
        ClassDefinitions definitions = new();

        Player player = new Player(){name = "am0hus"};
        Lobby lobby = new()
        {
            players = new Player[] { new Player() { name = "among us" } }
        };
        Serializer ser = new Serializer(metaInf);
            int ptr_=0;
            byte[] test = 
            {0x01, 0x01 , 0x00 , 0x05 , 0x01 , 0x00 
            , 0x03 , 0x05 , 0x00 , 0x4c , 0x6f , 0x62 
            , 0x62 , 0x79 , 0x02 , 0x01 , 0x00 , 0x07 
            , 0x1e , 0x06 , 0x07 , 0x00 , 0x70 , 0x6c 
            , 0x61 , 0x79 , 0x65 , 0x72 , 0x73 , 0xfd 
            , 0xfe , 0xff};
        var test_ = ClassDefinitions.FromBytes(test,ref ptr_,decoder).DEBUG();
        Console.WriteLine(test_);
        Console.WriteLine(lobby.players.GetType().GetElementType());

        var bts = ser.Serialize(lobby);
        Deserializer des = new(bts);
        Console.WriteLine("class dictionary:");
        Console.WriteLine(ser.Definitions.DEBUG());

        var json = JsonSerializer.Serialize(lobby, op);
        Console.WriteLine($"length : {bts.Length}");
        Console.WriteLine(ByteArrayToString(bts));
        Console.WriteLine($"json length : {json.Length}");
        Console.WriteLine(json);
        try
        {
            var obj = des.Deserialize();

            Console.WriteLine("deserialized object:");
            Console.WriteLine(JsonSerializer.Serialize(obj, op));
        }
        catch (Exception ex)
        {
            Console.WriteLine(des.Definitions.DEBUG());
            Console.WriteLine(ex);
        }

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
class Lobby
{
    public Player[] players;
}
class Player
{
    public string name;
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