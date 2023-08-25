// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text;
using ObjectByteConverter;
using System.ComponentModel.DataAnnotations;

class Program
{
    static void Main(string[] args)
    {
        Serializer s = new();
        string str = "$CN";
        var b1 = s.WriteString_UTF8(str);
        var b3 = s.WriteString(str);
        var b4 = s.Meta_WriteString(str);
        Console.WriteLine("UTF8   :"+ByteArrayToString(b1));
        Console.WriteLine("Unicode:"+ByteArrayToString(b3));
        Console.WriteLine("Meta   :"+ByteArrayToString(b4));
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