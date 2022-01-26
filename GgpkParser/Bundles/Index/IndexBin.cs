using GgpkParser.DataTypes;
using GgpkParser.Extensions;
using GgpkParser.Libs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GgpkParser.Bundles.Index
{
    public class IndexBin
    {
        public IndexBin(in Stream stream, Data data)
        {
            Length = data.Length;
            Offset = data.Offset;

            stream.Position = data.Offset;

            BundleCount = stream.Read<uint>();
            for (var i = 0; i < BundleCount; i++)
            {
                BundleInfos.Add(new BundleInfo(stream));
            }

            FileCount = stream.Read<uint>();
            for (var i = 0; i < FileCount; i++)
            {
                FileInfos.Add(new FileInfo(stream));
            }

            PathRepCount = stream.Read<uint>();
            for (var i = 0; i < PathRepCount; i++)
            {
                PathReps.Add(new PathRep(stream));
            }

            PathBundle = new Bundle(stream, new Data(stream.Position, Offset + Length - stream.Position));
            using var pathBundleMemory = new MemoryStream(PathBundle.Decompress(stream));

            for (var i = 0; i < PathRepCount; i++)
            {
                Paths.AddRange(PathReps[i].GeneratePaths(pathBundleMemory));
            }

            var fnv1a = new FNV1aHash64();
            HashToFileName = Paths.ToDictionary(x => BitConverter.ToInt64(fnv1a.ComputeHash(Encoding.UTF8.GetBytes($"{x.ToLowerInvariant()}++"))), x => x);

            for (var i = 0; i < FileCount; i++)
            {
                FileInfos[i].Path = HashToFileName[FileInfos[i].Hash];
            }
        }

        public long Length { get; }
        public long Offset { get; }

        public uint BundleCount { get; }
        public List<BundleInfo> BundleInfos { get; } = new List<BundleInfo>();
        public uint FileCount { get; }
        public List<FileInfo> FileInfos { get; } = new List<FileInfo>();

        public uint PathRepCount { get; }
        public List<PathRep> PathReps { get; } = new List<PathRep>();

        public Bundle PathBundle { get; }
        public List<string> Paths { get; } = new List<string>();

        public Dictionary<long, string> HashToFileName { get; } = new Dictionary<long, string>();
    }
}
