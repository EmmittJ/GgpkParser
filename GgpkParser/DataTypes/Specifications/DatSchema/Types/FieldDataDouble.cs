using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class FieldDataDouble : FieldData<double>
    {
        public FieldDataDouble() : base(FieldType.Double) { }
        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            Value = stream.Read<double>();
            return this;
        }
    }
}
