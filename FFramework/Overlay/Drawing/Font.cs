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

using SharpDX.DirectWrite;

using FontFactory = SharpDX.DirectWrite.Factory;

namespace FFramework.Overlay.Drawing
{
    public class Font : IDisposable
    {
        public TextFormat TextFormat;
        public bool Bold => TextFormat.FontWeight == FontWeight.Bold;
        public bool Italic => TextFormat.FontStyle == FontStyle.Italic;

        public bool WordWeapping
        {
            get => TextFormat.WordWrapping == WordWrapping.Wrap;
            set => TextFormat.WordWrapping = value ? WordWrapping.Wrap : WordWrapping.NoWrap;
        }
        
        public float FontSize => TextFormat.FontSize;
        public string FontFamilyName => TextFormat.FontFamilyName;

        private Font()
        {
            throw new NotImplementedException();
        }

        public Font(TextFormat textFormat)
        {
            TextFormat = textFormat ?? throw new ArgumentNullException();
        }

        public Font(FontFactory factory, string fontFamilyName, float size, bool bold = false, bool italic = false, bool wordWrapping = false)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (string.IsNullOrEmpty(fontFamilyName)) throw new ArgumentNullException(nameof(fontFamilyName));

            TextFormat = new TextFormat(factory, fontFamilyName, bold ? FontWeight.Bold : FontWeight.Normal, italic ? FontStyle.Italic : FontStyle.Normal, size)
            {
                WordWrapping = wordWrapping ? WordWrapping.Wrap : WordWrapping.NoWrap
            };
        }

        ~Font()
        {
            Dispose(false);
        }

        public override bool Equals(object obj)
        {
            var font = obj as Font;

            if (font == null)
            {
                return false;
            }
            else
            {
                return font.Bold == Bold
                    && font.Italic == Italic
                    && font.WordWeapping == WordWeapping
                    && font.FontSize == FontSize
                    && font.FontFamilyName == FontFamilyName;
            }
        }

        public bool Equals(Font value)
        {
            return value != null
                && value.Bold == Bold
                && value.Italic == Italic
                && value.WordWeapping == WordWeapping
                && value.FontSize == FontSize
                && value.FontFamilyName == FontFamilyName;
        }

        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                Bold.GetHashCode(),
                Italic.GetHashCode(),
                WordWeapping.GetHashCode(),
                FontSize.GetHashCode(),
                FontFamilyName.GetHashCode());
        }

        public override string ToString()
        {
            return OverrideHelper.ToString(
                "FontFamilyName", FontFamilyName,
                "FontSize", FontSize.ToString(),
                "Italic", Italic.ToString(),
                "Bold", Bold.ToString(),
                "WordWrapping", WordWeapping.ToString());
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (TextFormat != null) TextFormat.Dispose();

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        public static implicit operator TextFormat(Font font)
        {
            if (font == null) throw new ArgumentNullException(nameof(font));

            return font.TextFormat;
        }

        public static bool Equals(Font left, Font right)
        {
            return left != null
                && left.Equals(right);
        }
    }
}