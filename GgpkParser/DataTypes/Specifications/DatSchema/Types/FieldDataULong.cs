using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class FieldDataULong : FieldData<ulong>
    {
        public FieldDataULong() : base(FieldType.ULong) { }
        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            Value = stream.Read<ulong>();
            return this;
        }
    }
}
