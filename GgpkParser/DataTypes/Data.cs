namespace GgpkParser.DataTypes
{
    public class Data
    {
        public Data(long offset, long length)
        {
            Offset = offset;
            Length = length;
        }

        public long Offset { get; }
        public long Length { get; }
    }
}
