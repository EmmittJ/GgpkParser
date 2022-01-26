using GgpkParser.DataTypes.DatSchema;
using GgpkParser.DataTypes.Specifications.DatSchema.Types;
using GgpkParser.Extensions;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace GgpkParser.DataTypes.Specifications
{
    [Specification(FileExtension = ".dat", Priority = 1)]
    [Specification(FileExtension = ".dat64", Priority = 1)]
    [Specification(FileExtension = ".datl", Priority = 1)]
    [Specification(FileExtension = ".datl64", Priority = 1)]
    public class DatSpecification : IDataSpecification
    {
        private static readonly ulong DataSeparator = 0xbbbbbbbbbbbbbbbb;
        public byte[] RawData { get; private set; } = Array.Empty<byte>();
        public DataTable DataTable { get; private set; } = new DataTable();
        public string Name { get; }
        public SchemaTable? Table { get; set; }
        public int RowCount { get; private set; }

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "X64 looks bad.")]
        public bool x64 { get; private set; }
        public bool UTF32 { get; private set; }

        public DatSpecification(string name = "")
        {
            Name = name;
            var ext = Path.GetExtension(name);
            x64 = ext.EndsWith("64");
            UTF32 = ext.StartsWith(".datl");

            var nameWithoutExtension = Path.GetFileNameWithoutExtension(Name);
            if (SchemaFileService.Default.Tables.FirstOrDefault(x => x.Name == nameWithoutExtension) is SchemaTable table)
            {
                Table = table;
            }
        }

        public void LoadFrom(in Stream stream, Data data)
        {
            stream.Position = data.Offset;
            RawData = stream.Read<byte>(data.Length);

            if (Table is null)
            {
                return;
            }

            using var memory = new MemoryStream(RawData);

            RowCount = memory.Read<int>();
            var start = memory.IndexOf(DataSeparator, memory.Position);
            var rowSize = RowCount > 0 ? (start - memory.Position) / RowCount : 0;
            var size = Table.Columns.Sum(x => IFieldData.SizeOf(x.FieldData.FieldType, x64));

            if (rowSize != size)
            {
                Console.WriteLine($"{Name}: Expected Size {size} does match match Actual Size {rowSize}");
            }

            DataTable.Columns.Add(new DataColumn()
            {
                ColumnName = "#",
                Caption = "#",
                DataType = typeof(int),
                ReadOnly = true,
                AutoIncrement = true,
            });

            for (var i = 0; i < Table.Columns.Count; i++)
            {
                var column = Table.Columns[i];
                var name = column.Name ?? $"u{i}";
                DataTable.Columns.Add(new DataColumn()
                {
                    ColumnName = name,
                    Caption = $"{name}: {column.Type}",
                    DataType = column.FieldData.Type,
                    ReadOnly = true,
                });
            }

            for (var i = 0; i < RowCount; i++)
            {
                var row = DataTable.NewRow();
                for (var index = 0; index < Table.Columns.Count; index++)
                {
                    var column = Table.Columns[index];
                    var value = column.FieldData switch
                    {
                        _ when column.FieldData is IReferenceData reference => reference.Read(memory, start, x64, UTF32).Value,
                        _ when column.FieldData is IFieldData field => field.Read(memory, x64, UTF32).Value,
                        _ => null,
                    };

                    var name = column.Name ?? $"u{index}";
                    row[name] = value is not null ? value.ToString() : DBNull.Value;
                }
                DataTable.Rows.Add(row);
            }
        }
    }
}
