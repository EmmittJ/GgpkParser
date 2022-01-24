using GgpkParser.DataTypes.Specifications.DatSchema.Types;
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
        public IFieldData FieldData => IFieldData.From(this, Array, Type);

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
    }

    public class Reference
    {
        public string Table { get; set; } = string.Empty;
        public string? Column { get; set; }
    }
}
