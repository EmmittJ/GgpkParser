using GgpkParser.DataTypes;
using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.Bundles
{
    public class BundleHeader
    {
        public BundleHeader(in Stream stream)
        {
            UncompressedSize = stream.Read<uint>();
            TotalPayloadSize = stream.Read<uint>();
            PayloadSize = stream.Read<uint>();
        }

        public uint UncompressedSize { get; private set; }
        public uint TotalPayloadSize { get; private set; }
        public uint PayloadSize { get; private set; }
    }
}
