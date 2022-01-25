using GgpkParser.Records;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Buffers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

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

        public static unsafe object Read(this Stream stream, Type type) => type switch
        {
            _ when type == typeof(bool) => stream.Read<bool>(),
            _ when type == typeof(sbyte) => stream.Read<sbyte>(),
            _ when type == typeof(byte) => stream.Read<byte>(),
            _ when type == typeof(short) => stream.Read<short>(),
            _ when type == typeof(ushort) => stream.Read<ushort>(),
            _ when type == typeof(int) => stream.Read<int>(),
            _ when type == typeof(uint) => stream.Read<uint>(),
            _ when type == typeof(long) => stream.Read<long>(),
            _ when type == typeof(ulong) => stream.Read<ulong>(),
            _ when type == typeof(float) => stream.Read<float>(),
            _ when type == typeof(double) => stream.Read<double>(),
            _ => throw new NotImplementedException(),
        };

        public static unsafe T Read<T>(this Stream stream) where T : unmanaged
        {
            var buffer = new byte[Unsafe.SizeOf<T>()];
            stream.Read(buffer);
            fixed (byte* b = &buffer[0])
            {
                return *(T*)b;
            }
        }

        public static unsafe T[] Read<T>(this Stream stream, long size) where T : unmanaged => stream.Read<T>((int)size);
        public static unsafe T[] Read<T>(this Stream stream, int size) where T : unmanaged
        {
            var owner = SpanOwner<T>.Allocate(size);
            stream.Read(owner.Span.Cast<T, byte>());
            return owner.Span.ToArray();
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

        public static unsafe long IndexOf<T>(this Stream stream, T value, long start = 0) where T : unmanaged
        {
            var original = stream.Position;
            stream.Position = start;

            var current = stream.Read<T>();
            while (!EqualityComparer<T>.Default.Equals(current, value) && stream.Position < stream.Length)
            {
                current = stream.Read<T>();
                stream.Position = stream.Position - Unsafe.SizeOf<T>() + 1;
            }

            var last = stream.Position - 1;
            stream.Position = original;
            return EqualityComparer<T>.Default.Equals(current, value) ? last : -1;
        }
    }
}
