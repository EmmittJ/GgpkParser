using GgpkParser.Extensions;
using System.IO;

namespace GgpkParser.DataTypes.Specifications
{
    [DataSpecification(Priority = 0)]
    public class RawBytesSpecification : IDataSpecification
    {
        public byte[] RawData { get; private set; } = new byte[0];

        public void LoadFrom(in Stream stream, Data data)
        {
            stream.Position = data.Offset;
            RawData = stream.Read<byte>(data.Length);
        }
    }
}
