## What can `ObjectByteConverter` serialize?
- Objects of simple classes (that is classed composed only of only primitives)
- Objects of compund classes (that is classed composing both primitives and objects)
- Objects of classes refering itself that terminate with `null` (example : linked lists)
- *One dimensional* arrays of the above mentioned type of objects and primitive types

## MetaInf
- `MetaInf.SizeT` dictates how to read/write the length of any array and how to read/write the `class ID` of any user defined type.
- `MetaInf.ClassName` is the name of the objects's class
- `MetaInf.stringEncodingMode` determines the encoding used to serialize and deserialize strings
- `MetaInf.Length` is the length of the datagram produced after serializing

## How to use this library
### Serialization
1. Import `ByteConverter`.
2. Set up the meta inf
```
MetaInf meta = new()
{
    stringEncodingMode = StringEncodingMode.UTF8,
    SizeT = DataTypeID.UInt16
};
```
3. Instantiate `Serializer`
```
Serializer serializer = new Serializer(meta);
```
4. Serialize the objectL
   `var data = serializer.Serialize(obj);`

## Primitive types:
- Fixed sized primitive types
    - Integer (8,16,32 and 64 bit)
    - Unsigned Integer (8,16,32 and 64 bit)
    - Floating point number (32 and 64 bit)
    - Character
- Variable size primitive types
    - String
    - Array of integers (8,16,32 and 64 bit)
    - Array of unsigned integers (8,16,32 and 64 bit)
    - Array of floating point numbers (32 and 64 bit)
    - Array of characters (treated as a string)
    - Array of strings

### Integers and floating point numbers:
- Serialization of integers and floats is done by the `ByteConverter.PrimitiveEncoder` class using the `System.BitConverter` library.
- Deserialization of integers and floats is done by the `ByteConverter.PrimitiveDecoder` class using the `System.BitConverter` library.

### Strings
`ByteConverter` can serialize a `string` in 3 ways:
- ASCII
- UTF8
- Unicode