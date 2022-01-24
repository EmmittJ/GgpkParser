using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class FieldDataFloat : FieldData<float>
    {
        public FieldDataFloat() : base(FieldType.Float) { }
        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            Value = stream.Read<float>();
            return this;
        }
    }
}
