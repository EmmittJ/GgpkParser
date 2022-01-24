using GgpkParser.DataTypes.DatSchema;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public interface IFieldData
    {
        public const uint x32NullPointer = 0xfefefefe;
        public const ulong x64NullPointer = 0xfefefefefefefefe;

        public FieldType FieldType { get; }
        public object? Value { get; }
        public Type Type { get; }
        public IFieldData Read(in Stream stream, bool x64, bool UTF32);

        public static IFieldData From(TableColumn column, bool array, string value)
        {
            var fieldType = FieldTypeFromString(value);
            return fieldType switch
            {
                _ when array => new ReferenceDataArray(From(column, false, value)),
                FieldType.Unknown => new FieldDataUnknown(),
                FieldType.Bool => new FieldDataBool(),
                FieldType.SByte => new FieldDataSByte(),
                FieldType.Short => new FieldDataShort(),
                FieldType.Int => new FieldDataInt(),
                FieldType.Long => new FieldDataLong(),
                FieldType.Byte => new FieldDataByte(),
                FieldType.UShort => new FieldDataUShort(),
                FieldType.UInt => new FieldDataUInt(),
                FieldType.ULong => new FieldDataULong(),
                FieldType.Float => new FieldDataFloat(),
                FieldType.Double => new FieldDataDouble(),
                FieldType.String => new ReferenceDataString(),
                FieldType.Row => new FieldDataRow(),
                FieldType.ForeignRow => new FieldDataForeignRow(),
                FieldType.EnumRow => new FieldDataEnumRow(column),
                _ => throw new NotImplementedException(),
            };
        }

        public static FieldType FieldTypeFromString(string value)
        {
            foreach (var type in Enum.GetValues(typeof(FieldType)).OfType<FieldType>())
            {
                if (typeof(FieldType).GetField(type.ToString()) is not FieldInfo field)
                {
                    continue;
                }

                if (field.GetCustomAttribute<DescriptionAttribute>() is not DescriptionAttribute attribute)
                {
                    continue;
                }

                if (attribute.Description == value)
                {
                    return type;
                }
            }

            throw new NotSupportedException($"No FieldType mapping for {value} could be found.");
        }

        public static int SizeOf(FieldType type, bool x64) => type switch
        {
            FieldType.Unknown => 0,
            FieldType.Bool => 1,
            FieldType.SByte => 1,
            FieldType.Short => 2,
            FieldType.Int => 4,
            FieldType.Long => 8,
            FieldType.Byte => 1,
            FieldType.UShort => 2,
            FieldType.UInt => 4,
            FieldType.ULong => 8,
            FieldType.Float => 4,
            FieldType.Double => 8,
            FieldType.String => x64 ? 8 : 4,
            FieldType.Row => x64 ? 8 : 4,
            FieldType.ForeignRow => x64 ? 16 : 8,
            FieldType.EnumRow => 4,
            FieldType.Array => x64 ? 16 : 8,
            _ => 0,
        };
    }

    public enum FieldType
    {
        /// <summary>
        /// For Arrays of Unknown Type.
        /// </summary>
        [Description("array")]
        Unknown,
        [Description("bool")]
        Bool,
        [Description("i8")]
        SByte,
        [Description("i16")]
        Short,
        [Description("i32")]
        Int,
        [Description("i64")]
        Long,
        [Description("u8")]
        Byte,
        [Description("u16")]
        UShort,
        [Description("u32")]
        UInt,
        [Description("u64")]
        ULong,
        [Description("f32")]
        Float,
        [Description("f64")]
        Double,
        [Description("string")]
        String,
        [Description("array[]")]
        Array,
        [Description("row")]
        Row,
        [Description("foreignrow")]
        ForeignRow,
        [Description("enumrow")]
        EnumRow,
    }
}
