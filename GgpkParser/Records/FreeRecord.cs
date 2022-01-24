using GgpkParser.DataTypes;
using System.Collections.Generic;
using System.IO;

namespace GgpkParser.Records
{
    public class FreeRecord : IRecord
    {
        public FreeRecord(in Stream stream, in RecordHeader header, in IRecord? parent = null)
        {
            Parent = parent;
            if (parent is not null) parent.Children.Add(this);
            Length = header.Length;
            Type = header.Type;
            Offset = stream.Position - 8;
            
            var dataLength = Offset + Length - stream.Position;
            Data = new Data(stream.Position, dataLength);
            stream.Seek(dataLength, SeekOrigin.Current);
        }

        public IRecord? Parent { get; }
        public IList<IRecord> Children { get; } = new List<IRecord>();
        public int Length { get; }
        public RecordType Type { get; }
        public long Offset { get; }

        public long Next { get; }
        public Data Data { get; }

        public override string ToString() => $"Type: {Type}, DataLength: {Data.Length}";
    }
}
