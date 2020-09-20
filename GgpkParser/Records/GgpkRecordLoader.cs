using GgpkParser.Bundles;
using GgpkParser.Bundles.Index;
using GgpkParser.DataTypes;
using GgpkParser.DataTypes.Specifications;
using GgpkParser.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Transactions;

namespace GgpkParser.Records
{
    public class GgpkRecordLoader
    {
        private const string INDEX_BIN = "_.index.bin";

        public GgpkRecordLoader(string path)
        {
            Stream = File.OpenRead(path);
        }

        private FileStream Stream { get; }
        public IndexBin? IndexBin { get; private set; } = null;
        public Dictionary<RecordType, HashSet<IRecord>> Records { get; } = new Dictionary<RecordType, HashSet<IRecord>>();

        public void Load()
        {

            RecordHeader header;
            do
            {
                header = Stream.Read<RecordHeader>();
            } while (header.Type != RecordType.Ggpk);

            var ggpk = new GgpkRecord(Stream, header);
            if (!Records.ContainsKey(ggpk.Type))
            {
                Records[ggpk.Type] = new HashSet<IRecord>();
            }
            Records[ggpk.Type].Add(ggpk);
            var queue = new Queue<(long Offset, IRecord Parent)>();

            foreach (var child in ggpk.Entries)
            {
                queue.Enqueue((Offset: child, Parent: ggpk));
            }

            while (queue.Count > 0)
            {
                var (offset, parent) = queue.Dequeue();
                Stream.Position = offset;

                var record = Stream.ReadRecord(parent);
                if (record is null) continue;

                if (!Records.ContainsKey(record.Type))
                {
                    Records[record.Type] = new HashSet<IRecord>();
                }
                Records[record.Type].Add(record);

                if (record is DirectoryRecord directory)
                {
                    foreach (var (_, Offset) in directory.Entries)
                    {
                        queue.Enqueue((Offset, Parent: directory));
                    }
                }
                else if (record is FreeRecord free)
                {
                    if (free.Next > 0)
                    {
                        queue.Enqueue((Offset: free.Next, Parent: free));
                    }
                }
                else if (record is FileRecord file && file.Name == INDEX_BIN)
                {
                    var bundle = new Bundle(Stream, file.Data);
                    var bytes = bundle.Decompress(Stream);

                    using var memory = new MemoryStream(bytes);
                    IndexBin = new IndexBin(memory, new Data(0, bytes.Length));
                }
            }
        }

        public IDataSpecification LoadRecord(FileRecord record)
        {
            var spec = GgpkDataLoader.CreateDataSpecification(record.Name);
            spec.LoadFrom(Stream, record.Data);
            return spec;
        }

        public IDataSpecification LoadRecord(string path)
        {
            if (IndexBin is null)
            {
                return new RawBytesSpecification();
            }

            var info = IndexBin.FileInfos.First(x => x.Path == path);
            if (info is null)
            {
                return new RawBytesSpecification();
            }

            var bundleInfo = IndexBin.BundleInfos[info.BundleIndex];
            var bundleFile = Records[RecordType.File].OfType<FileRecord>().First(x => x.Name == bundleInfo.BundleName);

            var bundle = new Bundle(Stream, bundleFile.Data);
            using var memory = new MemoryStream(bundle.Decompress(Stream));
            
            var spec = GgpkDataLoader.CreateDataSpecification(path);
            spec.LoadFrom(memory, new Data(info.Offset, info.Length));
            return spec;
            //var spec = GgpkDataLoader.CreateDataSpecification(record.Name);
            //spec.LoadFrom(Stream, record.Data);
            //return spec;
        }
    }
}
