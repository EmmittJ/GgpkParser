using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class FieldDataLong : FieldData<long>
    {
        public FieldDataLong() : base(FieldType.Long) { }
        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            Value = stream.Read<long>();
            return this;
        }
    }
}
