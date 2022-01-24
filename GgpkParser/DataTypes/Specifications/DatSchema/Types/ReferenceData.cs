using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public abstract class ReferenceData<T> : FieldData<T>, IReferenceData
    {
        public ReferenceData(FieldType type) : base(type) { }

        public virtual IFieldData Read(in Stream stream, long data, bool x64, bool UTF32)
        {
            var offset = x64 ? stream.Read<long>() : stream.Read<int>();
            if (offset < 0 || data + offset > stream.Length)
            {
                Value = default;
                return this;
            }

            var prev = stream.Position;

            stream.Position = data + offset;
            Read(stream, x64, UTF32);

            stream.Position = prev;
            return this;
        }
    }
}
