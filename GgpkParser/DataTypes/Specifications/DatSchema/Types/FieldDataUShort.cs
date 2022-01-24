using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class FieldDataUShort : FieldData<ushort>
    {
        public FieldDataUShort() : base(FieldType.UShort) { }
        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            Value = stream.Read<ushort>();
            return this;
        }
    }
}
