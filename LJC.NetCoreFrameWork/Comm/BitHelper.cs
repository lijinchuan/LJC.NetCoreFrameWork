using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LJC.NetCoreFrameWork.Comm
{
    public static class BitHelper
    {
        public static byte[] ConvertToByteArray(BitArray bits)
        {
            byte[] bytes = new byte[(int)Math.Ceiling(bits.Count / 8.0)];
            bits.CopyTo(bytes, 0);
            return bytes;
        }

        public static byte ConvertToByte(BitArray bits)
        {
            if (bits.Count > 8)
            {
                throw new ArgumentException("bits");
            }
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }

    }
}
