using GgpkParser.Extensions;
using System.IO;
using System.Text;

namespace GgpkParser.Bundles.Index
{
    public class BundleInfo
    {
        public BundleInfo(in Stream stream)
        {
            NameLength = stream.Read<int>();
            Name = Encoding.ASCII.GetString(stream.Read<byte>(NameLength));
            UncompressedSize = stream.Read<int>();

            BundleName = Name.EndsWith(".bundle.bin") ? Name : $"{Name}.bundle.bin";
            FileName = Path.GetFileName(BundleName);
        }

        public int NameLength { get; }
        public string Name { get; }
        public string BundleName { get; }
        public string FileName { get; }
        public int UncompressedSize { get; }
    }
}
