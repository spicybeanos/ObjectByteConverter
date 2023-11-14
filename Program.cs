// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text;
using ByteConverter;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

class Program
{
    static void Main(string[] args)
    {
        MetaInf metaInf = new()
        {
            stringEncodingMode = StringEncodingMode.UTF8,
            SizeTReader = DataTypeIDs.UInt16
        };
        StandardEncoder encoder = new(metaInf);
        StandardDecoder decoder = new(metaInf);
        int ptr = 0; JsonSerializerOptions op = new() { WriteIndented = true };

        UserDefinitions ud = new UserDefinitions();
        var suc = ud.AddClass(typeof(Test1));
        var suc2 = ud.AddClass(typeof(Test2));
        
        var bts = ud.GenerateTypesDictionaryByte(encoder);
        Console.WriteLine(ByteArrayToString(bts));
        var res = UserDefinitions.FromBytes(bts, ref ptr, decoder);

        Console.WriteLine($"Success : {suc} {suc2}");
        Console.WriteLine(ByteArrayToString(bts));
        Console.WriteLine($"dict1 : {PrintDictionary(res.ClassIDDictionary)}");
        Console.WriteLine($"dict2 : {PrintDictionary(res.GlobalDefinitions)}");
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
    public static string PrintDictionary<K, V>(Dictionary<K, V> dict)
    {
        string res = "[\n";
        foreach (var k in dict.Keys)
        {
            res += $"\t{{{k}:{dict[k]}}},\n";
        }
        res += "]";
        return res;
    }
}


class Test1
{
    public int k = 10,r=3;
    public string s = "w";
}

class Test2
{
    public int h = 10;
}