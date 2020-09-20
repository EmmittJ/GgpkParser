using GgpkParser.Extensions;
using System.Collections.Generic;
using System.IO;

namespace GgpkParser.Records
{
    public class GgpkRecord : IRecord
    {
        public GgpkRecord(in Stream stream, in RecordHeader header, in IRecord? parent = null)
        {
            Parent = parent;
            if (!(parent is null)) parent.Children.Add(this);
            Length = header.Length;
            Type = header.Type;
            Offset = stream.Position - 8;

            Version = stream.Read<int>();
            Entries = new long[(Offset + Length - stream.Position) / sizeof(long)];
            for (var i = 0; i < Entries.Length; i++)
            {
                Entries[i] = stream.Read<long>();
            }
        }

        public IRecord? Parent { get; }
        public IList<IRecord> Children { get; } = new List<IRecord>();

        public int Length { get; }
        public RecordType Type { get; }
        public long Offset { get; }

        public int Version { get; }
        public long[] Entries { get; }

        public override string ToString() => $"Type: {Type}, Entries: {Entries.Length}";
    }
}
