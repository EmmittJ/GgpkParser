using GgpkParser.Extensions;
using System.IO;
using System.Text;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class ReferenceDataArray : ReferenceData<string>
    {
        private readonly IFieldData _fieldData;
        public ReferenceDataArray(IFieldData field) : base(FieldType.Array) => _fieldData = field;

        public override IFieldData Read(in Stream stream, long data, bool x64, bool UTF32)
        {
            var elements = x64 ? stream.Read<long>() : stream.Read<int>();
            var offset = data + (x64 ? stream.Read<long>() : stream.Read<int>());

            if (_fieldData is FieldDataUnknown)
            {
                Value = "[_]";
                return this;
            }

            var prev = stream.Position;
            var builder = new StringBuilder("[");

            for (var i = 0; i < elements; i++)
            {
                stream.Position = offset;
                if (_fieldData is IReferenceData referenceData)
                {
                    referenceData.Read(stream, data, x64, UTF32);
                }
                else
                {
                    stream.Position += i * IFieldData.SizeOf(_fieldData.FieldType, x64);
                    _fieldData.Read(stream, x64, UTF32);
                }

                builder.Append(_fieldData.Value);
                if (i < elements - 1)
                {
                    builder.Append(", ");
                }
            }

            builder.Append(']');
            Value = builder.ToString();
            stream.Position = prev;
            return this;
        }

        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            _fieldData.Read(stream, x64, UTF32);
            return this;
        }
    }
}
