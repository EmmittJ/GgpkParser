using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class FieldDataInt : FieldData<int>
    {
        public FieldDataInt() : base(FieldType.Int) { }
        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            Value = stream.Read<int>();
            return this;
        }
    }
}
