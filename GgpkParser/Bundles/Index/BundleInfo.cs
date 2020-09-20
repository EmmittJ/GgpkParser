using GgpkParser.Extensions;
using System.Collections.Generic;
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
        }

        public int NameLength { get; }
        public string Name { get; }
        public string BundleName => Name.EndsWith(".bundle.bin") ? Name : $"{Name}.bundle.bin";
        public int UncompressedSize { get; }
    }
}
