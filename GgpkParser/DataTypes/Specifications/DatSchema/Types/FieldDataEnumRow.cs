using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class FieldDataEnumRow : FieldData<string>
    {
        public FieldDataEnumRow() : base(FieldType.EnumRow) { }
        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            uint? value = stream.Read<uint>();
            if (value == IFieldData.x32NullPointer)
            {
                value = null;
            }

            //TODO: Map value to ENUM
            Value = $"{(value is not null ? value : "null")}";
            return this;
        }
    }
}
