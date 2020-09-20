﻿using GgpkParser.Bundles;
using System.IO;

namespace GgpkParser.DataTypes.Specifications
{
    [DataSpecification(FileExtension = ".bundle.bin", Priority = 1)]
    public class BundleSpecification : IDataSpecification
    {
        public byte[] RawData { get; private set; } = new byte[0];
        public void LoadFrom(in Stream stream, Data data)
        {
            var bundle = new Bundle(stream, data);
            RawData = bundle.Decompress(stream);
        }
    }
}
