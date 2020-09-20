using GgpkParser.DataTypes;
using GgpkParser.Records;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GgpkParser.Extensions
{
    public static class StreamExtensions
    {
        public static IRecord? ReadRecord(this Stream stream, IRecord? parent = null)
        {
            var header = stream.Read<RecordHeader>();
            switch (header.Type)
            {
                case RecordType.Ggpk:
                    return new GgpkRecord(stream, header, parent);
                case RecordType.Directory:
                    return new DirectoryRecord(stream, header, parent);
                case RecordType.Free:
                    return new FreeRecord(stream, header, parent);
                case RecordType.File:
                    return new FileRecord(stream, header, parent);
                default:
                    Console.WriteLine($"Unsupported RecordType: {header.Type}");
                    return null;
            }
        }

        public static unsafe T Read<T>(this Stream stream) where T : unmanaged
        {
            var buffer = new byte[Unsafe.SizeOf<T>()];
            stream.Read(buffer);
            fixed (byte* b = &buffer[0])
            {
                return *(T*)b;
            }
        }

        public static unsafe T[] Read<T>(this Stream stream, int size) where T : unmanaged
        {
            var result = new T[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = stream.Read<T>();
            }
            return result;
        }

        public static unsafe T[] Read<T>(this Stream stream, long size) where T : unmanaged
        {
            var result = new T[size];
            for (long i = 0; i < size; i++)
            {
                result[i] = stream.Read<T>();
            }
            return result;
        }

        public static unsafe T[] ReadUntil<T>(this Stream stream, T terminator, bool includeTerminator = false) where T : unmanaged
        {
            var result = new List<T>();
            var current = stream.Read<T>();
            while (!EqualityComparer<T>.Default.Equals(current, terminator))
            {
                result.Add(current);
                current = stream.Read<T>();
            }

            if (includeTerminator)
            {
                result.Add(current);
            }

            return result.ToArray();
        }
    }
}
