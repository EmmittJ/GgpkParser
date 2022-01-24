using GgpkParser.Bundles;
using GgpkParser.Bundles.Index;
using System.IO;

namespace GgpkParser.DataTypes.Specifications
{
    [Specification(FileExtension = "_.index.bin", Priority = 100)]
    public class IndexBinSpecification : IDataSpecification
    {
        public byte[] RawData { get; private set; } = new byte[0];
        public IndexBin? IndexBin { get; private set; }
        public string Name { get; }

        public IndexBinSpecification(string name = "") => Name = name;

        public void LoadFrom(in Stream stream, Data data)
        {
            var bundle = new Bundle(stream, data);
            RawData = bundle.Decompress(stream);

            using var memory = new MemoryStream(RawData);
            IndexBin = new IndexBin(memory, new Data(0, RawData.Length));
        }
    }
}
