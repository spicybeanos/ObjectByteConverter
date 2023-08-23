// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text;
using ObjectByteConverter;
class Program
{
    static void Main(string[] args)
    {
        Serializer ser = new();
        V v = new();
        var bts  = ser.SerializeProperties(v);
        var des = new Desializer(bts);
        Console.WriteLine("Length after serializing : {0}",bts.Length);
        Console.WriteLine($"Dump : {des.Dump().Value}");
    }
}

public class V : C
{
    public string str { get; set; }
    public int[] ints { get; set; }
}

public class C
{
    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }
}