using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class FieldDataRow : FieldData<string>
    {
        public FieldDataRow() : base(FieldType.Row) { }
        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            ulong? value;
            if (x64)
            {
                value = stream.Read<ulong>();
                if (value == IFieldData.x64NullPointer)
                {
                    value = null;
                }
            }
            else
            {
                value = stream.Read<uint>();
                if (value == IFieldData.x32NullPointer)
                {
                    value = null;
                }
            }

            Value = $"<{(value is not null ? value : "null")}>";
            return this;
        }
    }
}
