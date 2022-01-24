using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class FieldDataBool : FieldData<bool>
    {
        public FieldDataBool() : base(FieldType.Bool) { }
        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            Value = stream.Read<bool>();
            return this;
        }
    }
}
