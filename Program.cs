// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text;
using ByteConverter;
using System.ComponentModel.DataAnnotations;

class Program
{
    static void Main(string[] args)
    {
        
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