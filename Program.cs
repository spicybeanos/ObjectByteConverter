// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text;
using ByteConverter;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

class Program
{
    static void Main(string[] args)
    {
        Serializer s = new();
        Test t = new(1,2,43);
        string js = JsonSerializer.Serialize(t);
        var b1 = s.Serialize(t);
        int p = 0;
        Console.WriteLine("UTF8   :" + ByteArrayToString(b1));
        Console.WriteLine($"Json: {js}");
        Console.WriteLine($"Json count : {js.Length}");
        string cname = Desializer.GetClassName(b1,ref p);
        Console.WriteLine($"Class name : {cname}");
        p =0;
        var k = Desializer.Deserialize<Test2>(b1,ref p);
        Console.WriteLine($"Deserialized:{JsonSerializer.Serialize(k)}");
    }
    public static string ByteArrayToString(byte[] arrInput)
    {
        int i;
        StringBuilder sOutput = new StringBuilder(arrInput.Length);
        for (i = 0; i < arrInput.Length; i++)
        {
            sOutput.Append(arrInput[i].ToString("x2") + " ");
        }
        return sOutput.ToString();
    }
}
public class Test
{
    public int a { get; set; }
    public int b { get; set; }
    public int c { get; set; }
    public Test(int a, int b, int c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }
}
public class Test2{
    public string k {get;set;}
}