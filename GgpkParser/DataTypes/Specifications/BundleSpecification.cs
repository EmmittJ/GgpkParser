using GgpkParser.Bundles;
using System.IO;

namespace GgpkParser.DataTypes.Specifications
{
    [Specification(FileExtension = ".bundle.bin", Priority = 1)]
    public class BundleSpecification : IDataSpecification
    {
        public byte[] RawData { get; private set; } = new byte[0];
        public string Name { get; }

        public BundleSpecification(string name = "") => Name = name;

        public void LoadFrom(in Stream stream, Data data)
        {
            var bundle = new Bundle(stream, data);
            RawData = bundle.Decompress(stream);
        }
    }
}
