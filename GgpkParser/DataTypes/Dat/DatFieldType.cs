using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GgpkParser.DataTypes.Dat
{
    public class DatFieldType
    {
        public DatFieldType(string type)
        {
            if (!type.StartsWith("ref|"))
            {
                DefinedType = GetPrimitiveType(type);
                return;
            }

            var info = type.Split('|');
            IsReference = info[0] == "ref";
            IsList = info[1] == "list";
            DefinedType = GetPrimitiveType(info[^1]);
        }

        public Type DefinedType { get; set; }
        public bool IsReference { get; set; }
        public bool IsList { get; set; }
        public unsafe int Size => DefinedType switch
        {
            _ when IsReference && IsList => 8,
            _ when IsReference => 4,
            _ when DefinedType == typeof(bool) => Unsafe.SizeOf<bool>(),
            _ when DefinedType == typeof(sbyte) => Unsafe.SizeOf<sbyte>(),
            _ when DefinedType == typeof(byte) => Unsafe.SizeOf<byte>(),
            _ when DefinedType == typeof(short) => Unsafe.SizeOf<short>(),
            _ when DefinedType == typeof(ushort) => Unsafe.SizeOf<ushort>(),
            _ when DefinedType == typeof(int) => Unsafe.SizeOf<int>(),
            _ when DefinedType == typeof(uint) => Unsafe.SizeOf<uint>(),
            _ when DefinedType == typeof(long) => Unsafe.SizeOf<long>(),
            _ when DefinedType == typeof(ulong) => Unsafe.SizeOf<ulong>(),
            _ when DefinedType == typeof(float) => Unsafe.SizeOf<float>(),
            _ when DefinedType == typeof(double) => Unsafe.SizeOf<double>(),
            _ => 0,
        };

        private static Type GetPrimitiveType(string type) => type switch
        {
            "bool" => typeof(bool),
            "byte" => typeof(sbyte),
            "ubyte" => typeof(byte),
            "short" => typeof(short),
            "ushort" => typeof(ushort),
            "int" => typeof(int),
            "uint" => typeof(uint),
            "long" => typeof(long),
            "ulong" => typeof(ulong),
            "float" => typeof(float),
            "double" => typeof(double),
            "string" => typeof(string),
            _ => throw new InvalidCastException(),
        };
    }
}