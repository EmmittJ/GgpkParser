using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class FieldDataUInt : FieldData<uint>
    {
        public FieldDataUInt() : base(FieldType.UInt) { }
        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            Value = stream.Read<uint>();
            return this;
        }
    }
}
