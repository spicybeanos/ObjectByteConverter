﻿// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text;
using ByteConverter;
using System.ComponentModel.DataAnnotations;

class Program
{
    static void Main(string[] args)
    {
        StandardEncoder encoder = new(DataTypeIDs.UInt16,StringEncodingMode.UTF8);
        var bts = encoder.EncodePrimitive(null,DataTypeIDs.String_std);
        StandardDecoder decoder = new(encoder.EncoderMetaInf);
        int ptr = 0;
        Console.WriteLine(decoder.DecodePrimitive<string>(bts,ref ptr));
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
