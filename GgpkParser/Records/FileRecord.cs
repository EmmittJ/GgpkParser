using GgpkParser.DataTypes;
using GgpkParser.Extensions;
using System.Collections.Generic;
using System.IO;

namespace GgpkParser.Records
{
    public class FileRecord : IRecord
    {
        public FileRecord(in Stream stream, in RecordHeader header, in IRecord? parent = null)
        {
            Parent = parent;
            if (!(parent is null)) parent.Children.Add(this);
            Length = header.Length;
            Type = header.Type;
            Offset = stream.Position - 8;

            NameLength = stream.Read<int>();
            Hash = stream.Read<byte>(32);
            Name = new string(stream.ReadUntil('\0'));
            var dataLength = Offset + Length - stream.Position;
            Data = new Data(stream.Position, dataLength);
            stream.Seek(dataLength, SeekOrigin.Current);
        }

        public IRecord? Parent { get; }
        public IList<IRecord> Children { get; } = new List<IRecord>();
        public int Length { get; }
        public RecordType Type { get; }
        public long Offset { get; }

        public int NameLength { get; }
        public byte[] Hash { get; }
        public string Name { get; }
        public Data Data { get; }

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

        public override string ToString() => $"Type: {Type}, Name: {Name}, DataLength: {Data.Length}";
    }
}
