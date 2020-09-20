using System.Collections.Generic;

namespace GgpkParser.Records
{
    public interface IRecord
    {
        public IRecord? Parent { get; }
        public IList<IRecord> Children { get; }

        public int Length { get; }
        public RecordType Type { get; }
        public long Offset { get; }
    }
}