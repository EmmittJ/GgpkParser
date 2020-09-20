using GgpkParser.DataTypes;
using GgpkParser.Extensions;
using GgpkParser.Libs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GgpkParser.Bundles
{
    public class Bundle
    {
        private const int SafeSpace = 64;
        private const int MaxChunkSize = 256 * 1024;

        public Bundle(in Stream stream, Data data)
        {
            Length = data.Length;
            Offset = data.Offset;

            stream.Position = data.Offset;
            BundleHeader = new BundleHeader(stream);
            
            BundlePayloadOffset = stream.Position;
            BundlePayload = new BundlePayload(stream);

            BundleBlockOffset = stream.Position;
        }

        private byte[]? _decompressed = null;
        public byte[] Decompress(in Stream stream)
        {
            if (!(_decompressed is null))
            {
                return _decompressed;
            }

            stream.Position = BundleBlockOffset;

            var decompressed = new byte[BundleHeader.UncompressedSize];
            var buffer = new byte[MaxChunkSize + SafeSpace];
            var offset = 0;
            var last = BundlePayload.BlockCount - 1;

            for (var i = 0; i < BundlePayload.BlockCount; i++)
            {
                var compressed = stream.Read<byte>(BundlePayload.BlockSizes[i]);
                var size = i < last ? MaxChunkSize : ((int)BundleHeader.UncompressedSize - (last * MaxChunkSize));
                LibOoz.Ooz_Decompress(compressed, compressed.Length, buffer, size);
                Array.Copy(buffer, 0, decompressed, offset, size);
                offset += size;
            }

            _decompressed = decompressed;
            return _decompressed;
        }

        public long Length { get; }
        public long Offset { get; }
        public long BundlePayloadOffset { get; }
        public long BundleBlockOffset { get; }

        public BundleHeader BundleHeader { get; }
        public BundlePayload BundlePayload { get; }
    }
}
