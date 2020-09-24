using GgpkParser.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GgpkParser.Bundles.Index
{
    public class PathRep
    {
        public PathRep(in Stream stream)
        {
            Hash = stream.Read<ulong>();
            Offset = stream.Read<uint>();
            Length = stream.Read<uint>();
            RecursiveSize = stream.Read<uint>();
        }

        public List<string> GeneratePaths(in Stream stream)
        {
            stream.Position = Offset;

            var isBasePhase = false;
            var bases = new List<string>();
            var results = new List<string>();

            while (stream.Position < Offset + Length)
            {
                var command = stream.Read<int>();
                if (command == 0)
                {
                    isBasePhase = !isBasePhase;
                    if (isBasePhase)
                    {
                        bases.Clear();
                    }
                }
                else
                {
                    var path = Encoding.ASCII.GetString(stream.ReadUntil<byte>(0, false));
                    var index = command - 1;
                    if (index < bases.Count)
                    {
                        path = $"{bases[index]}{path}";
                    }

                    (isBasePhase ? bases : results).Add(path);
                }
            }

            return results;
        }

        public ulong Hash { get; }
        public uint Offset { get; }
        public uint Length { get; }
        public uint RecursiveSize { get; }
    }
}