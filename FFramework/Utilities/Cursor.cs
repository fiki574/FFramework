using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace FFramework.Utilities
{
    public class Cursor
    {
        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("User32.Dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point point);

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;
        }

        public static void SetPosition(int x, int y)
        {
            Point p = new Cursor.Point();
            p.X = Convert.ToInt16(x);
            p.Y = Convert.ToInt16(y);
            Cursor.ClientToScreen(default(IntPtr), ref p);
            Cursor.SetCursorPos(p.X, p.Y);
        }
    }
}
