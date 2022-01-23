using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GgpkParser.DataTypes.DatSchema
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SchemaFile
    {
        public int Version { get; set; }
        public int CreatedAt { get; set; }
        public IList<SchemaTable> Tables { get; set; } = new List<SchemaTable>();
        public IList<SchemaEnumeration> Enumerations { get; set; } = new List<SchemaEnumeration>();

        private string DebuggerDisplay => $"Version: {Version}, Tables: {Tables.Count}, Enumerations: {Enumerations.Count}";
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SchemaTable
    {
        public string Name { get; set; } = string.Empty;
        public IList<TableColumn> Columns { get; set; } = new List<TableColumn>();

        private string DebuggerDisplay => $"Name: {Name}, Columns: {Columns.Count}";
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SchemaEnumeration
    {
        public string Name { get; set; } = string.Empty;
        public int Indexing { get; set; }
        public IList<string?> Enumerators { get; set; } = new List<string?>();

        private string DebuggerDisplay => $"Name: {Name}, Enumerators: {Enumerators.Count}";
    }

    public class TableColumn
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool Array { get; set; }
        public bool Unique { get; set; }
        public bool Localized { get; set; }
        public string? Util { get; set; }
        public Reference References { get; set; } = new();
        public string? File { get; set; }
        public IList<string> Files { get; set; } = new List<string>();

        public Type DefinedType => GetPrimitiveType(Type);
        public bool IsReference => Type switch
        {
            "string" => true,
            "row" => true,
            "foreignrow" => true,
            "enumrow" => true,
            _ => false
        };
        public bool IsRowColumn => Type switch
        {
            "foreignrow" => true,
            _ => false
        };
        public bool IsList => Array || Type == "array";
        public unsafe int SizeForRow => DefinedType switch
        {
            _ when IsRowColumn => 8,
            _ when IsList => 8,
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
        public unsafe int Size => DefinedType switch
        {
            _ when IsRowColumn => 8,
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
            "i32" => typeof(int),
            "f32" => typeof(float),
            "string" => typeof(string),
            _ => typeof(uint)
        };
    }

    public class Reference
    {
        public string Table { get; set; } = string.Empty;
        public string? Column { get; set; }

    }
}
