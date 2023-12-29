## What can `ObjectByteConverter` serialize?
- Objects of simple classes (that is classed composed only of only primitives)
- Objects of compund classes (that is classed composing both primitives and objects)
- Objects of classes refering itself that terminate with `null` (example : linked lists)
- *One dimensional* arrays of the above mentioned type of objects and primitive types

## Meta info
- `MetaInf.SizeT` dictates how to read/write the length of any array and how to read/write the `classID` of any user defined type.

## Primitive types:
- Recognised primitive types
    - Integer (8,16,32 and 64 bit)
    - Unsigned Integer (8,16,32 and 64 bit)
    - Floating point number (32 and 64 bit)
    - Character
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