﻿using System;
using System.Runtime.InteropServices;

namespace GgpkParser.Libs
{
    public class LibOoz
    {
        [DllImport("libooz.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Ooz_Decompress(byte[] compressedContent, int compressedLength, byte[] decompressedContent, int decompressedSize,
            int fuzz, int crc, int verbose, byte[]? dst_base, int e, IntPtr cb, IntPtr cb_ctx, IntPtr scratch, int scratch_size, int threadPhase);

        public static int Ooz_Decompress(byte[] compressedContent, int compressedLength, byte[] decompressedContent, int decompressedSize)
            => Ooz_Decompress(compressedContent, compressedLength, decompressedContent, decompressedSize, 0, 0, 0, null, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, 0);
    }
}
