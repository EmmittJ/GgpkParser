using System;
using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public abstract class FieldData<T> : IFieldData
    {
        public Type Type => typeof(T);
        public virtual T? Value { get; protected set; } = default;
        public virtual FieldType FieldType { get; protected set; }

        public FieldData(FieldType type) => FieldType = type;

        object? IFieldData.Value => Value;
        public abstract IFieldData Read(in Stream stream, bool x64, bool UTF32);
    }
}
