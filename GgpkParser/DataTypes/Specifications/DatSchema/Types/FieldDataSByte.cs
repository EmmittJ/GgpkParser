using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class FieldDataSByte : FieldData<sbyte>
    {
        public FieldDataSByte() : base(FieldType.SByte) { }
        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            Value = stream.Read<sbyte>();
            return this;
        }
    }
}
