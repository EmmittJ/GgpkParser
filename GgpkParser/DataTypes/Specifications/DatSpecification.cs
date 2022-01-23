using GgpkParser.DataTypes.DatSchema;
using GgpkParser.Extensions;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace GgpkParser.DataTypes.Specifications
{
    [DataSpecification(FileExtension = ".dat", Priority = 1)]
    public class DatSpecification : IDataSpecification
    {
        private static readonly ulong DataSeparator = 0xbbbbbbbbbbbbbbbb;
        private static readonly ulong NoPointer = 0xfefefefefefefefe;
        public byte[] RawData { get; private set; } = new byte[0];
        public DataTable DataTable { get; private set; } = new DataTable();
        public string Name { get; }
        public SchemaTable? Table { get; set; }
        public SchemaEnumeration? Enumeration { get; set; }
        public int RowCount { get; private set; }

        public DatSpecification(string name = "")
        {
            Name = name;

            var nameWithoutExtension = Path.GetFileNameWithoutExtension(Name);
            if (DatSchemaSpecification.Default.Tables.FirstOrDefault(x => x.Name == nameWithoutExtension) is SchemaTable table)
            {
                Table = table;
            }

            if (DatSchemaSpecification.Default.Enumerations.FirstOrDefault(x => x.Name == nameWithoutExtension) is SchemaEnumeration enumeration)
            {
                Enumeration = enumeration;
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
            var size = Table.Columns.Sum(x => x.SizeForRow);
            var sizeDifference = rowSize - size;

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
                    DataType = column.IsList || column.IsRowColumn ? typeof(string) : column.DefinedType,
                    ReadOnly = true,
                });
            }

            for (var i = 0; i < RowCount; i++)
            {
                var row = DataTable.NewRow();
                for (var index = 0; index < Table.Columns.Count; index++)
                {
                    var column = Table.Columns[index];
                    var name = column.Name ?? $"u{index}";
                    var type = column;
                    object? value = null;
                    if (type.IsReference)
                    {
                        if (type.IsList)
                        {
                            var elements = memory.Read<uint>();
                            var offset = start + memory.Read<uint>();

                            var builder = new StringBuilder();
                            builder.Append("[");

                            if (type.DefinedType == typeof(string))
                            {
                                var current = memory.Position;
                                memory.Position = offset;

                                for (var j = 0; j < elements; j++)
                                {
                                    var innerOffset = start + memory.Read<uint>();
                                    var x = ReadReferenceValue(memory, type.DefinedType, innerOffset);
                                    if (string.IsNullOrWhiteSpace(x.ToString())) continue;

                                    builder.Append(x.ToString());
                                    if (j < elements - 1) builder.Append(", ");
                                }

                                memory.Position = current;
                            }
                            else
                            {
                                for (var j = 0; j < elements; j++)
                                {
                                    object? t = null;
                                    object? c = null;
                                    if (type.IsRowColumn)
                                    {
                                        t = ReadReferenceValue(memory, type.DefinedType, offset + (j * type.Size));
                                        c = ReadReferenceValue(memory, type.DefinedType, offset + (j * type.Size) + 4);
                                    }
                                    else
                                    {
                                        t = ReadReferenceValue(memory, type.DefinedType, offset + (j * type.Size));
                                    }

                                    if (t is not null && c is not null)
                                    {
                                        builder.Append($"<{t},{c}>");
                                    }
                                    else
                                    {
                                        builder.Append(t);
                                    }
                                    if (j < elements - 1) builder.Append(", ");
                                }
                            }

                            builder.Append("]");
                            value = builder.ToString();
                        }
                        else if (type.IsRowColumn)
                        {
                            var t = ReadValue(memory, type.DefinedType);
                            var c = ReadValue(memory, type.DefinedType);
                            value = $"<{t},{c}>";
                        }
                        else
                        {
                            value = ReadReferenceValue(memory, type.DefinedType, start + memory.Read<uint>());
                        }
                    }
                    else
                    {
                        value = ReadValue(memory, type.DefinedType);
                    }

                    row[name] = value ?? DBNull.Value;
                }
                DataTable.Rows.Add(row);
                if (memory.Position + sizeDifference <= memory.Length)
                    memory.Position += sizeDifference;
            }
        }

        private object? ReadReferenceValue(in Stream stream, Type type, long offset)
        {
            if (offset > stream.Length) return null;

            var current = stream.Position;
            stream.Position = offset;

            var value = ReadValue(stream, type);

            stream.Position = current;
            return value;
        }

        private object ReadValue(in Stream stream, Type type) => type == typeof(string) ? new string(stream.ReadUntil('\0')) : stream.Read(type);
    }
}
