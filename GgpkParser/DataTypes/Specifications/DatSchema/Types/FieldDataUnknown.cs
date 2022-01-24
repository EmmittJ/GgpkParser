using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class FieldDataUnknown : FieldData<object>
    {
        public FieldDataUnknown() : base(FieldType.Unknown) { }
        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            Value = stream.Read<uint>();
            return this;
        }
    }
}
