using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFramework.FUtilities
{
    ///<summary>Class used for converting input from little to big endian form</summary>
    public class LittleToBigEndian
    {
        public static long LTBE(long input)
        {
            return (long)(((ulong)LTBE((uint)((ulong)input & 0xFFFFFFFF)) << 32) | (ulong)LTBE((uint)(((ulong)input & 0xFFFFFFFF00000000) >> 32)));
        }

        public static ulong LTBE(ulong input)
        {
            return (ulong)(((ulong)LTBE((uint)(input & 0xFFFFFFFF)) << 32) | (ulong)LTBE((uint)((input & 0xFFFFFFFF00000000) >> 32)));
        }

        public static int LTBE(int input)
        {
            return ((input & 0xff) << 24) + ((input & 0xff00) << 8) + ((input & 0xff0000) >> 8) + ((input >> 24) & 0xff);
        }

        public static uint LTBE(uint input)
        {
            return ((input & 0xff) << 24) + ((input & 0xff00) << 8) + ((input & 0xff0000) >> 8) + ((input >> 24) & 0xff);
        }

        public static ushort LTBE(ushort input)
        {
            return (ushort)(((input >> 8) & 0xff) + ((input << 8) & 0xff00));
        }

        public static short LTBE(short input)
        {
            return (short)(((input >> 8) & 0xff) + ((input << 8) & 0xff00));
        }
    }
}
