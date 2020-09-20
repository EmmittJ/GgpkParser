using System.IO;

namespace GgpkParser.DataTypes
{
    public interface IDataSpecification
    {
        public byte[] RawData { get; }

        public void LoadFrom(in Stream stream, Data data);
    }
}
