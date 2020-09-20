using GgpkParser.DataTypes;
using GgpkParser.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GgpkParser.Bundles
{
    public class BundlePayload
    {
        public BundlePayload(in Stream stream)
        {
            CompressionType = (CompressionType)stream.Read<uint>();
            unk10 = stream.Read<uint>();
            UncompressedSize = stream.Read<ulong>();
            TotalPayloadSize = stream.Read<ulong>();
            BlockCount = stream.Read<int>();
            UncompressedBlockGranularity = stream.Read<uint>();
            unk28 = stream.Read<uint>(4);
            BlockSizes = stream.Read<uint>(BlockCount);
        }

        public CompressionType CompressionType { get; private set; }
        public uint unk10 { get; private set; }
        public ulong UncompressedSize { get; private set; }
        public ulong TotalPayloadSize { get; private set; }
        public int BlockCount { get; private set; }
        public uint UncompressedBlockGranularity { get; private set; }
        public uint[] unk28 { get; private set; }
        public uint[] BlockSizes { get; private set; }
    }

    public enum CompressionType
    {
        Kraken_6 = 8,
        Mermaid_A = 9,
        Leviathan_C = 13
    }
}
