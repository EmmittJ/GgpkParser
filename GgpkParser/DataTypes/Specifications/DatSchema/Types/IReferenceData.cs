using System.IO;

namespace GgpkParser.DataTypes.Specifications.DatSchema.Types
{
    public interface IReferenceData : IFieldData
    {
        public IFieldData Read(in Stream stream, long data, bool x64, bool UTF32);
    }
}
