using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.Bundles.Index
{
    public class FileInfo
    {
        public FileInfo(in Stream stream)
        {
            Hash = stream.Read<long>();
            BundleIndex = stream.Read<int>();
            Offset = stream.Read<uint>();
            Length = stream.Read<uint>();
        }

        public long Hash { get; }
        public string Path { get; set; } = string.Empty;
        public int BundleIndex { get; }
        public uint Offset { get; }
        public uint Length { get; }
    }
}
