using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class FieldDataByte : FieldData<byte>
    {
        public FieldDataByte() : base(FieldType.Byte) { }
        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            Value = stream.Read<byte>();
            return this;
        }
    }
}
