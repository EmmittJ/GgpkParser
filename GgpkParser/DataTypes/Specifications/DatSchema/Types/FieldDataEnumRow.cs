using GgpkParser.DataTypes.DatSchema;
using GgpkParser.Extensions;
using System.IO;
using System.Linq;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public class FieldDataEnumRow : FieldData<string>
    {
        private readonly TableColumn _column;
        public FieldDataEnumRow(TableColumn column) : base(FieldType.EnumRow) => _column = column;

        public override IFieldData Read(in Stream stream, bool x64, bool UTF32)
        {
            int? enumindex = stream.Read<int>();
            if (enumindex == IFieldData.x32NullPointer)
            {
                enumindex = null;
            }

            var value = enumindex?.ToString();
            if (_column.References is Reference reference)
            {
                var enumeration = SchemaFileService.Default.Enumerations.FirstOrDefault(x => x.Name == reference.Table);
                if (enumeration is not null && enumindex is not null)
                {
                    var index = (int)enumindex - enumeration.Indexing;
                    if (index < enumeration.Enumerators.Count && enumeration.Enumerators[index] is string enumvalue)
                    {
                        value = enumvalue;
                    }
                }
            }

            Value = $"{value ?? "null"}";
            return this;
        }
    }
}
