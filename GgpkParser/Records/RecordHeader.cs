namespace GgpkParser.Records
{
    public struct RecordHeader
    {
        public int Length;
        public RecordType Type;
    }

    /// <summary>
    /// GGPK header field: Type, 4 bytes converted to int
    /// for performance reasons.
    /// </summary>
    public enum RecordType
    {
        File = 1162627398, // ASCII 'FILE'
        Free = 1162170950, // ASCII 'FREE'
        Directory = 1380533328, // ASCII 'PDIR'
        Ggpk = 1263552327 // ASCII 'GGPK'
    }
}
