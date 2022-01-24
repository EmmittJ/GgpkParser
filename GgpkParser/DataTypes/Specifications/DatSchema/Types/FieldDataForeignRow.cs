using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class FieldDataForeignRow : FieldData<string>
    {
        public FieldDataForeignRow() : base(FieldType.ForeignRow) { }
        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            ulong? table;
            ulong? column;
            if (x64)
            {
                table = stream.Read<ulong>();
                if (table == IFieldData.x64NullPointer)
                {
                    table = null;
                }

                column = stream.Read<ulong>();
                if (column == IFieldData.x64NullPointer)
                {
                    column = null;
                }
            }
            else
            {
                table = stream.Read<uint>();
                if (table == IFieldData.x32NullPointer)
                {
                    table = null;
                }

                column = stream.Read<uint>();
                if (column == IFieldData.x32NullPointer)
                {
                    column = null;
                }
            }

            Value = $"<{(table is not null ? table : "null")},{(column is not null ? column : "null")}>";
            return this;
        }
    }
}
