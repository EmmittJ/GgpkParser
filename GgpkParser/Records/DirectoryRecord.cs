using GgpkParser.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GgpkParser.Records
{
    public class DirectoryRecord : IRecord
    {
        public DirectoryRecord(in Stream stream, in RecordHeader header, in IRecord? parent = null)
        {
            Parent = parent;
            if (!(parent is null)) parent.Children.Add(this);
            Length = header.Length;
            Type = header.Type;
            Offset = stream.Position - 8;

            NameLength = stream.Read<int>();
            EntryCount = stream.Read<int>();
            Hash = stream.Read<byte>(32);
            Name = Encoding.Unicode.GetString(stream.Read<byte>((NameLength - 1) * 2));
            stream.Read<char>(); //no-op '\0'
            Entries = new (uint Hash, long Offset)[EntryCount];
            for (var i = 0; i < EntryCount; i++)
            {
                Entries[i] = (Hash: stream.Read<uint>(), Offset: stream.Read<long>());
            }
        }

        public IRecord? Parent { get; }
        public IList<IRecord> Children { get; } = new List<IRecord>();
        public int Length { get; }
        public RecordType Type { get; }
        public long Offset { get; }

        public int NameLength { get; }
        public int EntryCount { get; }
        public byte[] Hash { get; }
        public string Name { get; }
        public (uint Hash, long Offset)[] Entries { get; }

        private string? _path = null;
        public string Path
        {
            get
            {
                if (_path is null)
                {
                    _path = Name;
                    if (Parent is DirectoryRecord directory && !string.IsNullOrWhiteSpace(directory.Path))
                    {
                        _path = $"{directory.Path}{System.IO.Path.AltDirectorySeparatorChar}{Name}";
                    }
                }

                return _path;
            }
        }

        public override string ToString() => $"Type: {Type}, Name: {Name}, Entries: {Entries.Length}";
    }

    public class DirectoryEntry
    {
        public DirectoryEntry(in uint hash, in long offset)
        {
            Hash = hash;
            Offset = offset;
        }

        public uint Hash { get; }
        public long Offset { get; }
    }
}
