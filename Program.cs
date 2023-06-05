// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text;
using ObjectByteConverter;
class Program
{
    static void Main(string[] args)
    {
        ObjectSerializer ser = new ObjectSerializer();
        V v = new V() { x = 0, y = 21, z = 32, str = "Hello world", ints = new int[] { 0, 543, 485, 882 } };
        var b = ser.SerializeProperties(v);
        //Console.WriteLine(JsonSerializer.Serialize(b));
        ObjectDesializer des = new ObjectDesializer(b);
        var d = des.Decode();
        Console.WriteLine(JsonSerializer.Serialize(d));
        Console.WriteLine($"{nameof(v.x)}");
        
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