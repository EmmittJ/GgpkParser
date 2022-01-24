using GgpkParser.Extensions;
using System.IO;
using System.Linq;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class ReferenceDataString : ReferenceData<string>
    {
        public ReferenceDataString() : base(FieldType.String) { }
        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            if (UTF32)
            {
                Value = string.Join(string.Empty, stream.ReadUntil(0).Select(x => char.ConvertFromUtf32(x)));
            }
            else
            {
                Value = new string(stream.ReadUntil('\0'));
            }
            return this;
        }
    }
}
