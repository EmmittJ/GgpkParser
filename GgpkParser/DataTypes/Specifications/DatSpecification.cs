using GgpkParser.DataTypes.Dat;
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
        public DatFileSpecificationJson? Specification { get; } = null;
        public int RowCount { get; private set; }

        public DatSpecification(string name = "")
        {
            Name = name;
            if (DatSpecificationJson.Default.ContainsKey(Name))
            {
                Specification = DatSpecificationJson.Default[Name];
            }
        }

        public void LoadFrom(in Stream stream, Data data)
        {
            stream.Position = data.Offset;
            RawData = stream.Read<byte>(data.Length);

            if (Specification is null)
            {
                return;
            }
            using var memory = new MemoryStream(RawData);

            RowCount = memory.Read<int>();
            var start = memory.IndexOf(DataSeparator, memory.Position);
            var rowSize = RowCount > 0 ? (start - memory.Position) / RowCount : 0;
            var size = Specification.Fields.Sum(x => x.Value.FieldType.Size);

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

            foreach (var (name, field) in Specification.Fields)
            {
                DataTable.Columns.Add(new DataColumn()
                {
                    ColumnName = name,
                    Caption = $"{name}: {field.Type}",
                    DataType = field.FieldType.IsList ? typeof(string) : field.FieldType.DefinedType,
                    ReadOnly = true,
                });
            }

            for (var i = 0; i < RowCount; i++)
            {
                var row = DataTable.NewRow();
                foreach (var (name, field) in Specification.Fields)
                {
                    object? value = null;
                    if (field.FieldType.IsReference)
                    {
                        if (field.FieldType.IsList)
                        {
                            var elements = memory.Read<uint>();
                            var offset = start + memory.Read<uint>();
                            
                            var builder = new StringBuilder();
                            builder.Append("[");
                            
                            if (field.FieldType.DefinedType == typeof(string))
                            {
                                var current = memory.Position;
                                memory.Position = offset;

                                for (var j = 0; j < elements; j++)
                                {
                                    var innerOffset = start + memory.Read<uint>();
                                    var x = ReadReferenceValue(memory, field.FieldType.DefinedType, innerOffset);
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
                                    var x = ReadReferenceValue(memory, field.FieldType.DefinedType, offset + (j * field.FieldType.Size));
                                    
                                    builder.Append(x);
                                    if (j < elements - 1) builder.Append(", ");
                                }
                            }

                            builder.Append("]");
                            value = builder.ToString();
                        }
                        else
                        {
                            value = ReadReferenceValue(memory, field.FieldType.DefinedType, start + memory.Read<uint>());
                        }
                    }
                    else
                    {
                        value = ReadValue(memory, field.FieldType.DefinedType);
                    }

                    row[name] = value ?? DBNull.Value;
                }
                DataTable.Rows.Add(row);
            }


        }

        private object ReadReferenceValue(in Stream stream, Type type, long offset)
        {
            var current = stream.Position;
            stream.Position = offset;

            var value = ReadValue(stream, type);

            stream.Position = current;
            return value;
        }

        private object ReadValue(in Stream stream, Type type) => type == typeof(string) ? new string(stream.ReadUntil('\0')) : stream.Read(type);
    }
}
