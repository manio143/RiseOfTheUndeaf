using System;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace RiseOfTheUndeaf.Core.Intrinsics
{
    public static class CRC32
    {
        public static uint Compute(string s) => Compute(Encoding.UTF8.GetBytes(s));
        public static uint Compute(byte[] bytes)
        {
            if (!Sse42.IsSupported)
            {
                throw new NotSupportedException("SSE4.2 is not supported");
            }

            uint crc = 0;
            int offset = 0;
            int len = bytes.Length;

            // x64 fast lane 8 bytes at a time
            if (Sse42.X64.IsSupported)
            {
                while (len >= 8)
                {
                    crc = (uint)Sse42.X64.Crc32(crc, BitConverter.ToUInt64(bytes, offset));
                    offset += 8;
                    len -= 8;
                }
            }

            while (len > 0)
            {
                crc = Sse42.Crc32(crc, bytes[offset]);
                offset++;
                len--;
            }

            return crc;
        }
    }
}
