/*
    MIT License

    Copyright (c) 2016-2019 michel-pi
    Copyright (c) 2010-2014 SharpDX - Alexandre Mutel

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using System.Text;

namespace FFramework.Overlay
{
    internal static class OverrideHelper
    {
        public static int HashCodes(params int[] hashCodes)
        {
            if (hashCodes == null) throw new ArgumentNullException(nameof(hashCodes));
            if (hashCodes.Length == 0) throw new ArgumentOutOfRangeException(nameof(hashCodes));

            unchecked
            {
                int hash = 17;

                foreach (int code in hashCodes)
                {
                    hash = hash * 23 + code;
                }

                return hash;
            }
        }

        public static string ToString(params string[] strings)
        {
            if (strings == null) throw new ArgumentNullException(nameof(strings));
            if (strings.Length == 0 || strings.Length % 2 != 0) throw new ArgumentOutOfRangeException(nameof(strings));

            StringBuilder sb = new StringBuilder(16);

            sb.Append("{ ");

            for (int i = 0; i < strings.Length - 1; i += 2)
            {
                string name = strings[i];
                string value = strings[i + 1];

                if (name == null)
                {
                    if (value == null)
                    {
                        sb.Append("null");
                    }
                    else
                    {
                        sb.Append(value);
                    }
                }
                else if (value == null)
                {
                    sb.Append(name + ": null");
                }
                else
                {
                    sb.Append(name + ": " + value);
                }

                sb.Append(", ");
            }

            sb.Length -= 2;

            sb.Append(" }");

            return sb.ToString();
        }
    }
}